<script lang="ts">
import Vue from "vue";
import { Component, Prop, Watch } from "vue-property-decorator";
import EventBus, { EventMessageName } from "@/eventbus";
import TimelineEntry, { DateGroup } from "@/models/timelineEntry";
import EntryCardTimelineComponent from "@/components/timeline/entrycard.vue";
import { DateWrapper } from "@/models/dateWrapper";
import TimelineFilter from "@/models/timelineFilter";

@Component({
    components: {
        EntryCardComponent: EntryCardTimelineComponent,
    },
})
export default class LinearTimelineComponent extends Vue {
    @Prop() private timelineEntries!: TimelineEntry[];
    @Prop({ default: 0 }) private totalEntries!: number;
    @Prop() private isVisible!: boolean;
    @Prop() private filter!: TimelineFilter;

    private filteredTimelineEntries: TimelineEntry[] = [];
    private visibleTimelineEntries: TimelineEntry[] = [];
    private currentPage = 1;
    private hasErrors = false;
    private eventBus = EventBus;
    private dateGroups: DateGroup[] = [];
    private hasFilter = false;

    @Watch("filter", { deep: true })
    private applyTimelineFilter() {
        this.hasFilter = TimelineFilter.hasFilter(this.filter);
        this.filteredTimelineEntries = this.timelineEntries.filter((entry) =>
            entry.filterApplies(this.filter)
        );
    }

    @Watch("timelineEntries")
    private refreshEntries() {
        this.applyTimelineFilter();
    }

    @Watch("visibleTimelineEntries")
    private onVisibleEntriesUpdate() {
        if (this.visibleTimelineEntries.length > 0) {
            if (this.isVisible) {
                this.eventBus.$emit(
                    EventMessageName.TimelinePageUpdate,
                    this.visibleTimelineEntries[0].date
                );
            }
        }

        this.dateGroups = DateGroup.createGroups(this.visibleTimelineEntries);
        this.dateGroups = DateGroup.sortGroup(this.dateGroups);
    }

    private created() {
        this.eventBus.$on(
            EventMessageName.CalendarDateEventClick,
            (eventDate: DateWrapper) => {
                this.setPageFromDate(eventDate);
                // Wait for next render cycle until the pages have been calculated and displayed
                this.$nextTick().then(() => {
                    this.focusOnDate(eventDate);
                });
            }
        );

        this.eventBus.$on(
            EventMessageName.CalendarMonthUpdated,
            (firstEntryDate: DateWrapper) => {
                this.setPageFromDate(firstEntryDate);
            }
        );

        this.eventBus.$on(
            EventMessageName.TimelineEntryAdded,
            (entry: TimelineEntry) => {
                this.onEntryAdded(entry);
            }
        );
    }

    private linkGen(pageNum: number) {
        return `?page=${pageNum}`;
    }

    private get numberOfPages(): number {
        let result = 1;
        if (this.filteredTimelineEntries.length > this.filter.pageSize) {
            result = Math.ceil(
                this.filteredTimelineEntries.length / this.filter.pageSize
            );
        }
        return result;
    }

    private getHeadingDate(date: DateWrapper): string {
        return date.format("LLL d, yyyy");
    }

    @Watch("currentPage")
    @Watch("filter.pageSize")
    @Watch("filteredTimelineEntries")
    private calculateVisibleEntries() {
        // Handle the current page being beyond the max number of pages
        if (this.currentPage > this.numberOfPages) {
            this.currentPage = this.numberOfPages;
        }
        // Get the section of the array that contains the paginated section
        let lowerIndex = (this.currentPage - 1) * this.filter.pageSize;
        let upperIndex = Math.min(
            this.currentPage * this.filter.pageSize,
            this.filteredTimelineEntries.length
        );
        this.visibleTimelineEntries = this.filteredTimelineEntries.slice(
            lowerIndex,
            upperIndex
        );
    }

    @Watch("filter.pageSize")
    private onEntriesPerPageChange() {
        this.currentPage = 1;
    }

    private getVisibleCount(): number {
        return this.visibleTimelineEntries.length;
    }

    private setPageFromDate(eventDate: DateWrapper) {
        let index = this.filteredTimelineEntries.findIndex(
            (entry) => entry.date === eventDate
        );
        this.currentPage = Math.floor(index / this.filter.pageSize) + 1;
    }

    private get timelineIsEmpty(): boolean {
        return this.filteredTimelineEntries.length == 0;
    }

    private onEntryAdded(entry: TimelineEntry) {
        this.$nextTick().then(() => {
            this.setPageFromDate(entry.date);
            this.$nextTick().then(() => {
                this.focusOnDate(entry.date);
            });
        });
    }

    private focusOnDate(date: DateWrapper) {
        const dateEpoch = date.fromEpoch();
        let container: HTMLElement[] = this.$refs[dateEpoch] as HTMLElement[];
        container[0].focus();
    }
}
</script>

<template>
    <div>
        <b-row class="no-print sticky-top sticky-offset">
            <b-col class="py-2">
                <b-pagination-nav
                    v-show="!timelineIsEmpty"
                    v-model="currentPage"
                    :link-gen="linkGen"
                    :number-of-pages="numberOfPages"
                    data-testid="pagination"
                    first-number
                    last-number
                    next-text="Next"
                    prev-text="Prev"
                    use-router
                ></b-pagination-nav>
            </b-col>
            <b-col class="py-2 col-12 col-sm-auto order-first order-sm-last">
                <slot name="month-list-toggle"></slot>
            </b-col>
        </b-row>
        <b-row v-if="!timelineIsEmpty" class="sticky-top sticky-line" />
        <b-row
            id="listControls"
            class="no-print"
            data-testid="displayCountText"
        >
            <b-col>
                Displaying {{ getVisibleCount() }} out of
                {{ totalEntries }} records
            </b-col>
        </b-row>
        <div id="timeData" data-testid="linearTimelineData">
            <b-row v-for="dateGroup in dateGroups" :key="dateGroup.key">
                <b-col cols="auto">
                    <div
                        :id="dateGroup.key"
                        :ref="dateGroup.key"
                        data-testid="dateGroup"
                        class="date"
                        tabindex="1"
                    >
                        {{ getHeadingDate(dateGroup.date) }}
                    </div>
                </b-col>
                <b-col>
                    <hr class="dateBreakLine" />
                </b-col>
                <EntryCardComponent
                    v-for="(entry, index) in dateGroup.entries"
                    :key="entry.type + '-' + entry.id"
                    :datekey="dateGroup.key"
                    :entry="entry"
                    :index="index"
                    data-testid="timelineCard"
                />
            </b-row>
        </div>
        <div v-if="timelineIsEmpty" class="text-center pt-2">
            <b-row>
                <b-col>
                    <img
                        class="mx-auto d-block"
                        src="@/assets/images/timeline/empty-state.png"
                        width="200"
                        height="auto"
                        alt="..."
                    />
                </b-col>
            </b-row>
            <b-row>
                <b-col>
                    <p
                        class="text-center pt-2 noTimelineEntriesText"
                        data-testid="noTimelineEntriesText"
                    >
                        <span v-if="hasFilter"
                            >No records found with the selected filters</span
                        >
                        <span v-else>No records found</span>
                    </p>
                </b-col>
            </b-row>
        </div>
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.column-wrapper {
    border: 1px;
}

.dateBreakLine {
    border-top: dashed 2px $primary;
}

.date {
    padding-top: 0px;
    color: $primary;
    font-size: 1.3em;
}

.sticky-offset {
    top: 70px;
    background-color: white;
    z-index: 2;
}

.sticky-line {
    top: 139px;
    background-color: white;
    border-bottom: solid $primary 2px;
    margin-top: -2px;
    z-index: 1;
    @media (max-width: 575px) {
        top: 193px;
    }
}

.noTimelineEntriesText {
    font-size: 1.5rem;
    color: #6c757d;
}
</style>
