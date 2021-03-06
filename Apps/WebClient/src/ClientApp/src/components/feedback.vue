<script lang="ts">
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { IUserFeedbackService } from "@/services/interfaces";
import User from "@/models/user";
import { IconDefinition, library } from "@fortawesome/fontawesome-svg-core";
import {
    faComments,
    faExclamationCircle,
    faMinus,
} from "@fortawesome/free-solid-svg-icons";
import {
    faCheckCircle,
    faTimesCircle as farTimesCircle,
} from "@fortawesome/free-regular-svg-icons";
library.add(faComments, faExclamationCircle, faMinus);

const sidebar = "sidebar";
const user = "user";

@Component
export default class FeedbackComponent extends Vue {
    @Action("toggleSidebar", { namespace: sidebar }) toggleSidebar!: () => void;

    @Getter("isOpen", { namespace: sidebar }) isSidebarOpen!: boolean;
    @Getter("user", { namespace: user }) user!: User;

    private comment = "";

    private visible = false;
    private hasSubmitted = false;
    private isSuccess = false;
    private isLoading = false;
    private userFeedbackService!: IUserFeedbackService;

    @Watch("isSidebarOpen")
    private onIsSidebarOpen(newValue: boolean) {
        // Make sure it closes if the sidebar is closing and reset state
        if (!newValue) {
            this.resetFeedback();
        }
    }

    private mounted() {
        this.userFeedbackService = container.get(
            SERVICE_IDENTIFIER.UserFeedbackService
        );
    }

    private get isValid() {
        return this.comment.length > 1;
    }

    private get resultTitle(): string {
        if (this.hasSubmitted) {
            return this.isSuccess ? "Awesome!" : "Oh no!";
        }

        return "";
    }

    private get resultIcon(): IconDefinition {
        return this.isSuccess ? faCheckCircle : farTimesCircle;
    }

    private get resultDescription(): string {
        if (this.hasSubmitted) {
            return this.isSuccess
                ? "Your message has been sent successfully!"
                : "Your Message could not be sent out!";
        }

        return "";
    }

    private get hasEmail(): boolean {
        return this.user.verifiedEmail && this.user.hasEmail;
    }

    private toggleExpanded() {
        this.visible = !this.visible;
        if (!this.isSidebarOpen) {
            this.toggleSidebar();
        }
    }

    private onSubmit(event: Event) {
        this.isLoading = true;

        this.userFeedbackService
            .submitFeedback({
                comment: this.comment,
            })
            .then((result) => {
                this.isSuccess = result;
            })
            .catch(() => {
                this.isSuccess = false;
            })
            .finally(() => {
                this.hasSubmitted = true;
                this.isLoading = false;
            });

        event.preventDefault();
    }

    private resetFeedback() {
        this.visible = false;
        this.hasSubmitted = false;
        this.isSuccess = false;
        this.comment = "";
    }
}
</script>

<template>
    <div
        id="feedback-container"
        class="d-flex flex-column text-dark"
        data-testid="feedbackContainer"
    >
        <b-button
            data-testid="expandFeedbackBtn"
            class="justify-content-center feedback py-0 rounded-0"
            :class="{
                'bg-danger': hasSubmitted && !isSuccess,
                'bg-success': hasSubmitted && isSuccess,
            }"
            :aria-expanded="visible ? 'true' : 'false'"
            aria-controls="collapse"
            size="sm"
            @click="toggleExpanded"
        >
            <b-row v-show="!hasSubmitted">
                <b-col
                    title="Feedback"
                    class="py-2"
                    :class="{
                        'col-2 col-md-4 text-right pr-2': isSidebarOpen,
                    }"
                >
                    <font-awesome-icon
                        icon="comments"
                        aria-hidden="true"
                        size="1x"
                        class="m-0"
                    />
                </b-col>
                <b-col
                    v-show="isSidebarOpen"
                    class="button-title text-left col-7 col-md-6 p-0 pl-3 py-2"
                >
                    <span> Feedback </span>
                </b-col>
                <b-col v-show="isSidebarOpen && visible" cols="auto">
                    <font-awesome-icon
                        icon="minus"
                        aria-hidden="true"
                        size="1x"
                        class="m-0 minimize-icon"
                    />
                </b-col>
            </b-row>
            <b-row v-show="hasSubmitted">
                <b-col>
                    <font-awesome-icon
                        :icon="resultIcon"
                        aria-hidden="true"
                        size="2x"
                        class="m-0"
                    />
                </b-col>
            </b-row>
        </b-button>
        <b-collapse id="collapse" v-model="visible" class="input-container">
            <div v-if="!hasSubmitted">
                <b-row class="p-3 description-container">
                    <b-col class="text-left small"
                        ><span class="button-title"
                            >Do you have a suggestion or idea? Let us know in
                            the field below.</span
                        >
                    </b-col>
                </b-row>
                <b-form ref="feedbackForm" @submit.prevent="onSubmit">
                    <b-row class="px-0">
                        <b-col>
                            <b-form-textarea
                                id="comment"
                                v-model="comment"
                                data-testid="feedbackCommentInput"
                                size="sm"
                                placeholder="Describe your suggestion or idea..."
                                rows="5"
                                max-rows="3"
                                maxlength="500"
                                class="border-0 p-2"
                                :disabled="isSuccess || isLoading"
                            />
                        </b-col>
                    </b-row>
                    <b-row class="submit-button-container my-3">
                        <b-col class="d-flex justify-content-center">
                            <b-button
                                v-if="!isSuccess && !isLoading"
                                data-testid="sendFeedbackMessageBtn"
                                size="md"
                                class="aqua-button px-5"
                                type="submit"
                                :disabled="!isValid"
                                >Send Message</b-button
                            >
                            <b-spinner v-if="isLoading"></b-spinner>
                        </b-col>
                    </b-row>
                </b-form>
            </div>
            <!-- Request result section -->
            <div v-else>
                <b-row>
                    <b-col class="my-3 text-center">
                        <p class="font-weight-bold">
                            {{ resultTitle }}
                        </p>
                        <p class="small">
                            {{ resultDescription }}
                        </p>
                    </b-col>
                </b-row>
                <b-row v-if="isSuccess && !hasEmail" class="mb-4">
                    <b-col class="col-3 text-right p-0 m-0">
                        <font-awesome-icon
                            icon="exclamation-circle"
                            aria-hidden="true"
                            size="2x"
                            class="m-0 text-warning"
                        />
                    </b-col>
                    <b-col class="text-left px-3 small" cols="9">
                        <span>
                            We won't be able to respond to your message unless
                            you have a verified email address in your profile.
                        </span>
                    </b-col>
                </b-row>
                <!-- Result buttons -->
                <b-row v-if="isSuccess && !hasEmail" class="mb-4">
                    <b-col class="p-0 ml-auto mr-2" cols="auto">
                        <b-button
                            data-testid="noNeedBtn"
                            variant="link"
                            size="sm"
                            @click="resetFeedback"
                            >No Need!</b-button
                        >
                    </b-col>
                    <b-col class="p-0 mr-auto ml-2" cols="auto">
                        <router-link
                            id="menuBtnProfile"
                            data-testid="menuBtnProfileLink"
                            to="/profile"
                        >
                            <b-button
                                data-testid="updateMyEmailButton"
                                size="sm"
                                class="aqua-button"
                                >Update my email
                            </b-button>
                        </router-link>
                    </b-col>
                </b-row>
                <b-row v-else>
                    <b-col>
                        <b-button
                            v-if="isSuccess && hasEmail"
                            data-testid="hasEmailResetFeedbackBtn"
                            size="md"
                            class="aqua-button px-5 bg-success mb-4"
                            :disabled="isLoading"
                            @click="resetFeedback"
                            >Got it!</b-button
                        >
                        <b-button
                            v-if="!isSuccess"
                            data-testid="tryAgainBtn"
                            size="md"
                            class="aqua-button px-5 bg-danger mb-4"
                            :disabled="isLoading"
                            @click="onSubmit($event)"
                            >Try Again</b-button
                        >
                    </b-col>
                </b-row>
            </div>
        </b-collapse>
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.feedback {
    background-color: $aquaBlue;
    display: inline;
    font-size: 1.3em;
    border: 0px;
    transition: all 0.3s;
}

.input-container {
    background-color: $lightGrey;
    border: 0px;
    transition: all 0.3s;
}

.aqua-button {
    background-color: $aquaBlue;
    border: 0px;
}

.description-container {
    height: 70px;
}

.submit-button-container {
    height: 36px;
    overflow: hidden;
}
.minimize-icon {
    font-size: 0.8em;
}
</style>
