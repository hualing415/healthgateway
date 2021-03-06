<script lang="ts">
import Vue from "vue";
import LoadingComponent from "@/components/loading.vue";
import { Component, Emit } from "vue-property-decorator";
import { Validation } from "vuelidate/vuelidate";
import { sameAs, required, minLength } from "vuelidate/lib/validators";
import { DateWrapper } from "@/models/dateWrapper";
import { Duration } from "luxon";
import { IDependentService } from "@/services/interfaces";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import ErrorTranslator from "@/utility/errorTranslator";
import BannerError from "@/models/bannerError";
import { Action, Getter } from "vuex-class";
import { ResultError } from "@/models/requestResult";
import AddDependentRequest from "@/models/addDependentRequest";
import User from "@/models/user";

export enum GenderType {
    NotSelected = "",
    Male = "Male",
    Female = "Female",
    Other = "NotSpecified",
}

@Component({
    components: {
        LoadingComponent,
    },
})
export default class NewDependentComponent extends Vue {
    @Getter("user", { namespace: "user" }) user!: User;

    @Action("addError", { namespace: "errorBanner" })
    addError!: (error: BannerError) => void;

    private dependentService!: IDependentService;
    private isVisible = false;
    private isLoading = true;
    private dependent: AddDependentRequest = {
        firstName: "",
        lastName: "",
        dateOfBirth: "",
        PHN: "",
        gender: GenderType.NotSelected,
    };
    private accepted = false;
    private agreement = `
        I confirm that I am the parent or
        guardian for this child, pursuant to
        Part 4, Division 3 of the Family Law
        Act, the Adoption Act, and/or the Child,
        Family and Community Services
        Act.
    `;
    private genderOptions = [
        { value: GenderType.NotSelected, text: "Please select an option" },
        { value: GenderType.Male, text: "Male" },
        { value: GenderType.Female, text: "Female" },
        { value: GenderType.Other, text: "Unknown" },
    ];

    private validations() {
        return {
            dependent: {
                firstName: {
                    required: required,
                },
                lastName: {
                    required: required,
                },
                dateOfBirth: {
                    required: required,
                    minLength: minLength(10),
                    minValue: (value: string) =>
                        new DateWrapper(value).toJSDate() > this.minBirthdate,
                },
                gender: {
                    required: required,
                },
                PHN: {
                    required: required,
                    minLength: minLength(12),
                },
            },
            accepted: { isChecked: sameAs(() => true) },
        };
    }

    private isValid(param: Validation): boolean | undefined {
        return param.$dirty ? !param.$invalid : undefined;
    }

    private get minBirthdate(): Date {
        let mindate = new DateWrapper()
            .subtract(Duration.fromObject({ years: 19 }))
            .toJSDate();
        return mindate;
    }

    public showModal(): void {
        this.isVisible = true;
    }

    public hideModal(): void {
        this.$v.$reset();
        this.isVisible = false;
    }

    private mounted() {
        this.dependentService = container.get<IDependentService>(
            SERVICE_IDENTIFIER.DependentService
        );
        this.isLoading = false;
    }

    private handleOk(bvModalEvt: Event) {
        // Prevent modal from closing
        bvModalEvt.preventDefault();
        this.$v.$touch();
        if (this.$v.$invalid) {
        } else {
            this.$v.$reset();
            this.addDependent();
        }
    }

    private addDependent() {
        this.dependentService
            .addDependent(this.user.hdid, {
                ...this.dependent,
                PHN: this.dependent.PHN.replace(/\D/g, ""),
            })
            .then(() => {
                this.handleSubmit();
            })
            .catch((err: ResultError) => {
                this.addError(
                    ErrorTranslator.toBannerError(
                        "Error adding dependent. Please review fields and try again.",
                        err
                    )
                );
                this.handleSubmit();
            })
            .finally(() => {
                this.dependent = {
                    firstName: "",
                    lastName: "",
                    dateOfBirth: "",
                    PHN: "",
                    gender: GenderType.NotSelected,
                };
                this.accepted = false;
            });
    }

    @Emit()
    private handleSubmit() {
        // Hide the modal manually
        this.$nextTick(() => {
            this.hideModal();
        });
    }
}
</script>

<template>
    <b-modal
        id="new-dependent-modal"
        v-model="isVisible"
        data-testid="newDependentModal"
        content-class="mt-5"
        title="Dependent Registration"
        size="lg"
        header-bg-variant="primary"
        header-text-variant="light"
        centered
    >
        <b-row>
            <b-col>
                <form>
                    <b-row data-testid="newDependentModalText">
                        <b-col>
                            <b-row>
                                <b-col class="col-12 col-md-4 mb-2">
                                    <label for="firstName">First Name</label>
                                    <b-form-input
                                        id="firstName"
                                        v-model="dependent.firstName"
                                        data-testid="firstNameInput"
                                        placeholder="John"
                                        :state="isValid($v.dependent.firstName)"
                                        @blur.native="
                                            $v.dependent.firstName.$touch()
                                        "
                                    ></b-form-input>
                                    <b-form-invalid-feedback
                                        :state="isValid($v.dependent.firstName)"
                                    >
                                        First name is required
                                    </b-form-invalid-feedback>
                                </b-col>
                                <b-col class="col-12 col-md-4 mb-2">
                                    <label for="lastName">Last Name</label>
                                    <b-form-input
                                        id="lastName"
                                        v-model="dependent.lastName"
                                        data-testid="lastNameInput"
                                        placeholder="Doe"
                                        :state="isValid($v.dependent.lastName)"
                                        @blur.native="
                                            $v.dependent.lastName.$touch()
                                        "
                                    ></b-form-input>
                                    <b-form-invalid-feedback
                                        :state="isValid($v.dependent.lastName)"
                                    >
                                        Last name is required
                                    </b-form-invalid-feedback>
                                </b-col>
                                <b-col class="col-12 col-md-4 mb-2">
                                    <label for="dateOfBirth"
                                        >Date of Birth</label
                                    >
                                    <b-form-input
                                        id="dateOfBirth"
                                        v-model="dependent.dateOfBirth"
                                        data-testid="dateOfBirthInput"
                                        required
                                        type="date"
                                        :state="
                                            isValid($v.dependent.dateOfBirth)
                                        "
                                    />
                                    <b-form-invalid-feedback
                                        :state="
                                            isValid($v.dependent.dateOfBirth)
                                        "
                                    >
                                        Dependent must be under the age of 19
                                    </b-form-invalid-feedback>
                                </b-col>
                            </b-row>
                            <b-row class="mb-2">
                                <b-col class="col-12 col-md-6 mb-2">
                                    <label for="phn">PHN</label>
                                    <b-form-input
                                        id="phn"
                                        v-model="dependent.PHN"
                                        v-mask="'#### ### ###'"
                                        data-testid="phnInput"
                                        placeholder="1234 567 890"
                                        :state="isValid($v.dependent.PHN)"
                                        @blur.native="$v.dependent.PHN.$touch()"
                                    ></b-form-input>
                                    <b-form-invalid-feedback
                                        :state="isValid($v.dependent.PHN)"
                                    >
                                        Valid PHN is required
                                    </b-form-invalid-feedback>
                                </b-col>
                                <b-col class="col-12 col-md-6 mb-2">
                                    <b-row>
                                        <b-col>
                                            <label for="gender">Gender</label>
                                            <b-form-select
                                                id="gender"
                                                v-model="dependent.gender"
                                                data-testid="genderInput"
                                                :options="genderOptions"
                                                :state="
                                                    isValid($v.dependent.gender)
                                                "
                                                @blur.native="
                                                    $v.dependent.gender.$touch()
                                                "
                                            >
                                            </b-form-select>
                                            <b-form-invalid-feedback
                                                :state="
                                                    isValid($v.dependent.gender)
                                                "
                                            >
                                                Please select from one of the
                                                options
                                            </b-form-invalid-feedback>
                                        </b-col>
                                    </b-row>
                                </b-col>
                            </b-row>
                            <b-row class="mb-2">
                                <b-col
                                    ><b-checkbox
                                        id="termsCheckbox"
                                        v-model="accepted"
                                        data-testid="termsCheckbox"
                                        :state="isValid($v.accepted)"
                                        >{{ agreement }}</b-checkbox
                                    ></b-col
                                >
                            </b-row>
                        </b-col>
                    </b-row>
                </form>
            </b-col>
        </b-row>
        <template #modal-footer>
            <b-row>
                <div class="mr-2">
                    <b-btn
                        data-testid="registerDependentBtn"
                        variant="primary"
                        @click="handleOk"
                        >Register dependent</b-btn
                    >
                </div>
                <div>
                    <b-btn
                        data-testid="cancelRegistrationBtn"
                        variant="secondary"
                        @click="hideModal"
                        >Cancel</b-btn
                    >
                </div>
            </b-row>
        </template>
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
    </b-modal>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
</style>
