<script lang="ts">
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";

import { ILogger } from "@/services/interfaces";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { ResultType } from "@/constants/resulttype";
import type { WebClientConfiguration } from "@/models/configData";
import User from "@/models/user";
import TimelineEntry from "@/models/timelineEntry";
import MedicationTimelineEntry from "@/models/medicationTimelineEntry";
import MedicationStatementHistory from "@/models/medicationStatementHistory";
import RequestResult from "@/models/requestResult";

import TimelineLoadingComponent from "@/components/timelineLoading.vue";
import ProtectiveWordComponent from "@/components/modal/protectiveWord.vue";
import ErrorCardComponent from "@/components/errorCard.vue";

import LineChartComponent from "@/components/timeline/plot/lineChart.vue";
import BannerError from "@/models/bannerError";
import ErrorTranslator from "@/utility/errorTranslator";
import { DateWrapper } from "@/models/dateWrapper";

const namespace = "user";

@Component({
    components: {
        TimelineLoadingComponent,
        ProtectiveWordComponent,
        ErrorCard: ErrorCardComponent,
        LineChart: LineChartComponent,
    },
})
export default class HealthInsightsView extends Vue {
    @Getter("user", { namespace }) user!: User;

    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;

    @Action("getMedicationStatements", { namespace: "medication" })
    getMedicationStatements!: (params: {
        hdid: string;
        protectiveWord?: string;
    }) => Promise<RequestResult<MedicationStatementHistory[]>>;

    @Action("addError", { namespace: "errorBanner" })
    addError!: (error: BannerError) => void;

    private logger!: ILogger;

    private timelineEntries: TimelineEntry[] = [];
    private isMedicationLoading = false;
    private protectiveWordAttempts = 0;

    private startDate: DateWrapper | null = null;
    private endDate: DateWrapper | null = null;

    private timeChartData: Chart.ChartData = {};

    private chartOptions: Chart.ChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
    };

    @Ref("protectiveWordModal")
    readonly protectiveWordModal!: ProtectiveWordComponent;

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        this.fetchMedicationStatements();
    }

    private get isLoading(): boolean {
        return this.isMedicationLoading;
    }

    private get isMedicationEnabled(): boolean {
        return this.config.modules["Medication"];
    }

    private get visibleCount(): number {
        return this.timelineEntries.length;
    }

    private fetchMedicationStatements(protectiveWord?: string) {
        this.isMedicationLoading = true;

        this.getMedicationStatements({
            hdid: this.user.hdid,
            protectiveWord: protectiveWord,
        })
            .then((results) => {
                if (results.resultStatus == ResultType.Success) {
                    this.protectiveWordAttempts = 0;
                    // Add the medication entries to the timeline list
                    for (let result of results.resourcePayload) {
                        this.timelineEntries.push(
                            new MedicationTimelineEntry(result)
                        );
                    }
                    this.timelineEntries = this.timelineEntries.reverse();
                    this.startDate = this.timelineEntries[0].date;
                    this.endDate = this.timelineEntries[
                        this.timelineEntries.length - 1
                    ].date;

                    this.prepareMonthlyChart();
                } else if (results.resultStatus == ResultType.Protected) {
                    this.protectiveWordModal.showModal();
                    this.protectiveWordAttempts++;
                } else {
                    this.logger.error(
                        "Error returned from the medication statements call: " +
                            results.resultError?.resultMessage
                    );
                    this.addError(
                        ErrorTranslator.toBannerError(
                            "Fetch Medications Error",
                            results.resultError
                        )
                    );
                }
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError(
                        "Fetch Medications Error",
                        err
                    )
                );
            })
            .finally(() => {
                this.isMedicationLoading = false;
            });
    }

    private onProtectiveWordSubmit(value: string) {
        this.fetchMedicationStatements(value);
    }

    private onProtectiveWordCancel() {
        // Does nothing as it won't be able to fetch pharmanet data.
        this.logger.debug("protective word cancelled");
    }

    /**
     * Gets an array of months between two dates
     */
    private getMonthsBetweenDates(
        startDate: DateWrapper,
        endDate: DateWrapper
    ): string[] {
        if (endDate.isBefore(startDate)) {
            throw "End date must be greated than start date.";
        }

        var result: string[] = [];
        while (startDate.isBefore(endDate)) {
            result.push(startDate.format("yyyy-LL-01"));
            startDate = startDate.add({ months: 1 });
        }
        return result;
    }

    /**
     * Prepares the dataset based on the timeline entries
     */
    private prepareMonthlyChart() {
        let months: string[] = this.getMonthsBetweenDates(
            this.startDate as DateWrapper,
            this.endDate as DateWrapper
        );

        let entryIndex = 0;
        let monthCounter: number[] = [];
        for (let monthIndex = 0; monthIndex < months.length; monthIndex++) {
            let currentMonth = months[monthIndex];
            monthCounter.push(0);

            while (entryIndex < this.timelineEntries.length) {
                let entry = this.timelineEntries[entryIndex];
                if (new DateWrapper(currentMonth).isSame(entry.date, "month")) {
                    monthCounter[monthIndex] += 1;
                    entryIndex++;
                } else {
                    break;
                }
            }
        }

        this.timeChartData.labels = [];
        this.timeChartData.datasets = [
            {
                label: "Monthly medications count",
                backgroundColor: "#ff6666",
                data: [],
            },
        ];
        this.timeChartData.datasets[0].data = [];

        for (let i = 0; i < months.length; i++) {
            this.timeChartData.labels.push(
                new DateWrapper(months[i]).format("LLL yyyy")
            );
            this.timeChartData.datasets[0].data.push(monthCounter[i]);
        }
    }

    private getReadableDate(date: DateWrapper | null): string {
        if (date === null) {
            return "N/A";
        }

        return date.format("LLL yyyy");
    }
}
</script>

<template>
    <div>
        <TimelineLoadingComponent v-if="isLoading"></TimelineLoadingComponent>
        <b-row class="my-3 fluid">
            <b-col
                id="healthInsights"
                class="col-12 col-md-10 col-lg-9 column-wrapper"
            >
                <div id="pageTitle">
                    <h1 id="subject">Health Insights</h1>
                    <hr />
                </div>
                <div>
                    <h3>Medication count over time</h3>
                    <b-row class="py-4">
                        <b-col cols="auto">
                            <strong>First month with data: </strong>
                            {{ getReadableDate(startDate) }}
                        </b-col>
                        <b-col cols="auto">
                            <strong>Last month with data: </strong
                            >{{ getReadableDate(endDate) }}
                        </b-col>
                        <b-col cols="auto">
                            <strong>Total records: </strong
                            ><span data-testid="totalRecordsText">{{
                                visibleCount
                            }}</span>
                        </b-col>
                    </b-row>
                    <b-row v-if="!isLoading && visibleCount > 0">
                        <b-col>
                            <LineChart
                                :chartdata="timeChartData"
                                :options="chartOptions"
                            />
                        </b-col>
                    </b-row>
                    <b-row
                        v-if="!isLoading && visibleCount === 0"
                        class="text-center pt-5"
                    >
                        <b-col> No medication records </b-col>
                    </b-row>
                    <b-row v-if="isLoading">
                        <b-col>
                            <content-placeholders>
                                <content-placeholders-img />
                                <content-placeholders-text :lines="1" />
                            </content-placeholders>
                        </b-col>
                    </b-row>
                </div>
            </b-col>
        </b-row>
        <ProtectiveWordComponent
            ref="protectiveWordModal"
            :error="protectiveWordAttempts > 1"
            :is-loading="isLoading"
            @submit="onProtectiveWordSubmit"
            @cancel="onProtectiveWordCancel"
        />
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.column-wrapper {
    border: 1px;
}

#pageTitle {
    color: $primary;
}

#pageTitle hr {
    border-top: 2px solid $primary;
}
</style>
