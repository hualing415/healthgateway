<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Getter } from "vuex-class";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faSlidersH } from "@fortawesome/free-solid-svg-icons";
import { Emit, Prop, Watch } from "vue-property-decorator";
import { ILogger } from "@/services/interfaces";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import EventBus, { EventMessageName } from "@/eventbus";
import type { WebClientConfiguration } from "@/models/configData";
import TimelineFilter, { EntryTypeFilter } from "@/models/timelineFilter";
import { EntryType } from "@/models/timelineEntry";
library.add(faSlidersH);

@Component
export default class FilterComponent extends Vue {
    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;
    @Getter("isOpen", { namespace: "sidebar" }) isSidebarOpen!: boolean;

    @Prop() private medicationCount!: number;
    @Prop() private immunizationCount!: number;
    @Prop() private encounterCount!: number;
    @Prop() private laboratoryCount!: number;
    @Prop() private noteCount!: number;
    @Prop() private isListView!: boolean;

    private logger!: ILogger;
    private eventBus = EventBus;
    private isVisible = false;
    private windowWidth = 0;
    private steps = [25, 50, 100, 500];
    private stepIndex = 0;
    private filter: TimelineFilter = new TimelineFilter();
    private selectedEntryTypes: EntryType[] = [];

    private get isMobileView(): boolean {
        return this.windowWidth < 576;
    }

    private get enabledEntryTypes(): EntryTypeFilter[] {
        return this.filter.entryTypes.filter(
            (filter: EntryTypeFilter) => filter.isEnabled
        );
    }

    @Watch("selectedEntryTypes")
    private onSelectedEntryTypesChanged() {
        this.filter.entryTypes.forEach((et: EntryTypeFilter) => {
            et.isSelected = this.selectedEntryTypes.some(
                (set) => set == et.type
            );
        });
    }

    @Watch("isMobileView")
    private onIsMobileView() {
        this.isVisible = false;
    }

    @Watch("isSidebarOpen")
    private onIsSidebarOpen() {
        this.isVisible = false;
    }

    @Watch("filter", { deep: true })
    private onFilterUpdate() {
        this.filtersChanged();
    }

    @Watch("noteCount")
    private noteCountUpdate(newCount: number) {
        this.filter.entryTypes[4].numEntries = newCount;
    }

    @Emit()
    private filtersChanged(): TimelineFilter {
        return this.filter;
    }

    private sliderChanged() {
        this.filter.pageSize = this.steps[this.stepIndex];
        this.filtersChanged();
    }

    private created() {
        window.addEventListener("resize", this.handleResize);
        this.handleResize();
    }

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        this.clearFilters();

        this.eventBus.$on(
            EventMessageName.SelectedFilter,
            (filterType: EntryType) => {
                this.onExternalFilterSelection(filterType);
            }
        );
        this.eventBus.$on(EventMessageName.TimelineEntryAdded, () => {
            this.onEntryAdded();
        });
    }

    private get hasFilterSelected(): boolean {
        return (
            this.selectedEntryTypes.length > 0 ||
            this.filter.startDate !== "" ||
            this.filter.endDate !== ""
        );
    }

    private destroyed() {
        window.removeEventListener("handleResize", this.handleResize);
    }

    private handleResize() {
        this.windowWidth = window.innerWidth;
    }

    private toggleMobileView() {
        this.isVisible = !this.isVisible;
    }

    private clearFilters(): void {
        this.selectedEntryTypes = [];
        this.filter = {
            keyword: "",
            pageSize: this.filter.pageSize,
            startDate: "",
            endDate: "",
            entryTypes: [
                {
                    type: EntryType.Immunization,
                    display: "Immunizations",
                    isEnabled: this.config.modules[EntryType.Immunization],
                    numEntries: this.immunizationCount,
                    isSelected: false,
                },
                {
                    type: EntryType.Medication,
                    display: "Medications",
                    isEnabled: this.config.modules[EntryType.Medication],
                    numEntries: this.medicationCount,
                    isSelected: false,
                },

                {
                    type: EntryType.Laboratory,
                    display: "Laboratory",
                    isEnabled: this.config.modules[EntryType.Laboratory],
                    numEntries: this.laboratoryCount,
                    isSelected: false,
                },
                {
                    type: EntryType.Encounter,
                    display: "MSP Visits",
                    isEnabled: this.config.modules[EntryType.Encounter],
                    numEntries: this.encounterCount,
                    isSelected: false,
                },
                {
                    type: EntryType.Note,
                    display: "My Notes",
                    isEnabled: this.config.modules[EntryType.Note],
                    numEntries: this.noteCount,
                    isSelected: false,
                },
            ],
        };
    }

    private onExternalFilterSelection(filterType: EntryType) {
        this.clearFilters();
        this.selectedEntryTypes.push(filterType);
    }

    private formatFilterCount(num: number): string {
        return Math.abs(num) > 999
            ? parseFloat(
                  ((Math.round(num / 100) * 100) / 1000).toFixed(1)
              ).toString() + "K"
            : num.toString();
    }

    private onEntryAdded() {
        this.clearFilters();
    }
}
</script>

<template>
    <div class="filters-wrapper">
        <div class="filters-width d-none d-sm-block">
            <b-dropdown
                data-testid="filterDropdown"
                text="Filter"
                class="w-100"
                :toggle-class="{ 'filter-selected': hasFilterSelected }"
                menu-class="z-index-large w-100"
                variant="outline-primary"
                right
            >
                <b-row class="px-4">
                    <b-col><strong>Type</strong> </b-col>
                    <b-col class="col-auto">
                        <b-button
                            variant="link"
                            class="p-0 m-0 btn-mobile"
                            @click="clearFilters()"
                        >
                            Clear
                        </b-button>
                    </b-col>
                </b-row>
                <div class="px-4">
                    <b-row
                        v-for="(filter, index) in enabledEntryTypes"
                        :key="index"
                    >
                        <b-col cols="8" align-self="start">
                            <b-form-checkbox
                                :id="filter.type + '-filter'"
                                v-model="selectedEntryTypes"
                                :data-testid="`${filter.type}-filter`"
                                :name="filter.type + '-filter'"
                                :value="filter.type"
                            >
                                {{ filter.display }}
                            </b-form-checkbox>
                        </b-col>
                        <b-col
                            cols="4"
                            align-self="end"
                            class="text-right"
                            :data-testid="`${filter.type}Count`"
                        >
                            ({{ formatFilterCount(filter.numEntries) }})
                        </b-col>
                    </b-row>
                    <b-row class="mt-2">
                        <b-col><strong>Dates</strong> </b-col>
                        <b-col class="col-auto"></b-col>
                    </b-row>
                    <b-row class="mt-1">
                        <b-col>
                            <b-form-input
                                id="start-date"
                                v-model="filter.startDate"
                                data-testid="filterStartDateInput"
                                type="date"
                            />
                        </b-col>
                    </b-row>
                    <b-row class="mt-1">
                        <b-col>
                            <b-form-input
                                id="end-date"
                                v-model="filter.endDate"
                                data-testid="filterEndDateInput"
                                type="date"
                            />
                        </b-col>
                    </b-row>
                    <b-row v-if="isListView" class="mt-2">
                        <b-col>
                            <label for="entries-per-page"
                                >Items per page: {{ steps[stepIndex] }}</label
                            >
                            <b-form-input
                                id="entries-per-page"
                                v-model="stepIndex"
                                type="range"
                                min="0"
                                max="3"
                                @change="sliderChanged()"
                            ></b-form-input>
                        </b-col>
                    </b-row>
                </div>
            </b-dropdown>
        </div>

        <!-- Mobile view specific modal-->
        <b-button
            class="d-d-sm-inline d-sm-none"
            :class="{ 'filter-selected': hasFilterSelected }"
            variant="outline-primary"
            @click.stop="toggleMobileView"
        >
            <font-awesome-icon icon="sliders-h" aria-hidden="true" size="1x" />
        </b-button>
        <b-modal
            id="generic-message"
            v-model="isVisible"
            title="Filter"
            content-class="filters-mobile-content"
            header-bg-variant="outline"
            :hide-footer="true"
            no-fade
        >
            <template #modal-header="{ close }">
                <b-row class="w-100 text-center p-0 m-0">
                    <b-col class="col-3">
                        <!-- Emulate built in modal header close button action -->
                        <b-button
                            variant="link"
                            class="m-0 p-0 btn-mobile btn-close"
                            @click="close()"
                        >
                            <font-awesome-icon
                                icon="times"
                                aria-hidden="true"
                                size="1x"
                                class="m-0"
                            />
                        </b-button> </b-col
                    ><b-col class="col-6 pt-1">
                        <h5>Filter</h5>
                    </b-col>
                    <b-col class="col-3 pt-1">
                        <b-button
                            variant="link"
                            class="p-0 m-0 btn-mobile"
                            @click="clearFilters()"
                        >
                            Clear
                        </b-button>
                    </b-col>
                </b-row>
            </template>
            <b-row class="justify-content-center py-2">
                <b-col class="col-10">
                    <h5>Type</h5>
                    <b-row
                        v-for="(filter, index) in enabledEntryTypes"
                        :key="index"
                    >
                        <b-col cols="8" align-self="start">
                            <b-form-checkbox
                                :id="filter.type + '-filter'"
                                v-model="selectedEntryTypes"
                                :data-testid="`${filter.type}-filter`"
                                :name="filter.type + '-filter'"
                                :value="filter"
                            >
                                {{ filter.display }}
                            </b-form-checkbox>
                        </b-col>
                        <b-col cols="4" align-self="end" class="text-right">
                            ({{ formatFilterCount(filter.numEntries) }})
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
            <b-row class="justify-content-center py-2">
                <b-col class="col-10">
                    <h5>Dates</h5>
                    <b-row>
                        <b-col>
                            <b-form-input
                                id="start-date"
                                v-model="filter.startDate"
                                data-testid="filterStartDateInput"
                                type="date"
                            />
                        </b-col>
                    </b-row>
                    <b-row class="mt-1">
                        <b-col>
                            <b-form-input
                                id="end-date"
                                v-model="filter.endDate"
                                data-testid="filterEndDateInput"
                                type="date"
                            />
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
            <b-row v-if="isListView" class="justify-content-center mt-2 mx-3">
                <b-col>
                    <label for="entries-per-page"
                        >Items per page: {{ steps[stepIndex] }}</label
                    >
                    <b-form-input
                        id="entries-per-page"
                        v-model="stepIndex"
                        type="range"
                        min="0"
                        max="3"
                        @change="sliderChanged()"
                    ></b-form-input>
                </b-col>
            </b-row>
        </b-modal>
    </div>
</template>

<style lang="scss" scoped>
.filters-wrapper {
    z-index: 3;
}
.filters-width {
    width: 225px;
}
</style>
<style lang="scss">
.filters-mobile-content {
    position: fixed;
    top: auto;
    right: auto;
    border: 0px;
    left: 0;
    bottom: 0;
    border-radius: 0px;
    .btn-mobile {
        color: #494949;
        border: none;
    }
    .btn-close {
        font-size: 1.5em;
    }
}
.filter-selected {
    border-width: 3px;
}
</style>
