﻿<script lang="ts">
import Vue from "vue";
import { Component, Emit, Prop, Watch } from "vue-property-decorator";

@Component
export default class CovidModalComponent extends Vue {
    @Prop() error!: boolean;
    @Prop({ default: false }) isLoading!: boolean;

    public isVisible = false;
    public show = false;

    public showModal(): void {
        this.show = true;
        if (!this.isLoading) {
            this.isVisible = true;
        }
    }

    public hideModal(): void {
        this.show = false;
        this.isVisible = false;
    }

    @Watch("isLoading")
    private onIsLoading() {
        if (!this.isLoading && this.show) {
            this.isVisible = true;
        }
    }

    @Emit()
    private submit() {
        this.show = false;
        this.isVisible = false;
        return;
    }

    @Emit()
    private cancel() {
        this.hideModal();
        return;
    }

    private handleSubmit(bvModalEvt: Event) {
        // Prevent modal from closing
        bvModalEvt.preventDefault();

        // Trigger submit handler
        this.submit();

        // Hide the modal manually
        this.$nextTick(() => {
            this.hideModal();
        });
    }

    private handleCancel(bvModalEvt: Event) {
        // Prevent modal from closing
        bvModalEvt.preventDefault();

        // Trigger cancel handler
        this.cancel();

        // Hide the modal manually
        this.$nextTick(() => {
            this.hideModal();
        });
    }
}
</script>

<template>
    <b-modal
        id="covid-modal"
        v-model="isVisible"
        data-testid="covidModal"
        title="COVID-19"
        header-bg-variant="danger"
        header-text-variant="light"
        footer-class="modal-footer"
        :no-close-on-backdrop="true"
        centered
        @close="cancel"
    >
        <b-row>
            <b-col>
                <form @submit.stop.prevent="handleSubmit">
                    <b-row data-testid="covidModalText">
                        <b-col>
                            <span
                                >Check the status of your COVID-19 test and view
                                your result when it is available</span
                            >
                        </b-col>
                    </b-row>
                </form>
            </b-col>
        </b-row>
        <template #modal-footer>
            <b-row>
                <b-col>
                    <b-row>
                        <b-col>
                            <b-button
                                data-testid="covidViewResultBtn"
                                variant="outline-primary"
                                @click="handleSubmit($event)"
                            >
                                View Result
                            </b-button>
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
        </template>
    </b-modal>
</template>

<style lang="scss" scoped>
.modal-footer {
    justify-content: flex-end;
}
</style>
