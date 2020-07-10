<style scoped lang="scss">
.error-message {
    color: #ff5252 !important;
}
</style>
<template>
    <v-data-table
        :headers="headers"
        :items="communicationList"
        :custom-sort="customSort"
        class="elevation-1"
    >
        <template v-slot:item.effectiveDateTime="{ item }">
            <span>{{ formatDate(item.effectiveDateTime) }}</span>
        </template>
        <template v-slot:item.expiryDateTime="{ item }">
            <span>{{ formatDate(item.expiryDateTime) }}</span>
        </template>
        <template v-slot:top>
            <v-toolbar dark>
                <v-toolbar-title>Communications</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" max-width="500px">
                    <template v-slot:activator="{ on, attrs }">
                        <v-btn color="primary" dark v-bind="attrs" v-on="on"
                            >New Banner Communication</v-btn
                        >
                    </template>
                    <v-card dark>
                        <v-card-title>
                            <span class="headline">{{ formTitle }}</span>
                        </v-card-title>
                        <v-card-text>
                            <v-form ref="form" lazy-validation>
                                <v-row>
                                    <v-col>
                                        <ValidationProvider
                                            v-slot="{
                                                errors
                                            }"
                                            :rules="
                                                dateTimeRules(
                                                    editedItem.effectiveDateTime,
                                                    editedItem.expiryDateTime
                                                )
                                            "
                                        >
                                            <v-datetime-picker
                                                v-model="
                                                    editedItem.effectiveDateTime
                                                "
                                                requried
                                                label="Effective On"
                                            ></v-datetime-picker>
                                            <span class="error-message">{{
                                                errors[0]
                                            }}</span>
                                        </ValidationProvider>
                                    </v-col>
                                    <v-col>
                                        <ValidationProvider
                                            v-slot="{
                                                errors
                                            }"
                                            :rules="
                                                dateTimeRules(
                                                    editedItem.effectiveDateTime,
                                                    editedItem.expiryDateTime
                                                )
                                            "
                                        >
                                            <v-datetime-picker
                                                v-model="
                                                    editedItem.expiryDateTime
                                                "
                                                required
                                                label="Expires On"
                                            ></v-datetime-picker>
                                            <span class="error-message">{{
                                                errors[0]
                                            }}</span>
                                        </ValidationProvider>
                                    </v-col>
                                </v-row>
                                <v-row>
                                    <v-col>
                                        <v-text-field
                                            v-model="editedItem.subject"
                                            label="Subject"
                                            maxlength="100"
                                            :rules="[
                                                v =>
                                                    !!v || 'Subject is required'
                                            ]"
                                            validate-on-blur
                                            required
                                        ></v-text-field>
                                    </v-col>
                                </v-row>
                                <v-row>
                                    <v-col>
                                        <v-textarea
                                            v-model="editedItem.text"
                                            label="Message"
                                            maxlength="1000"
                                            :rules="[
                                                v => !!v || 'Text is required'
                                            ]"
                                            validate-on-blur
                                            required
                                        ></v-textarea>
                                    </v-col>
                                </v-row>
                            </v-form>
                        </v-card-text>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue darken-1" text @click="close()"
                                >Cancel</v-btn
                            >
                            <v-btn color="blue darken-1" text @click="save()"
                                >Save</v-btn
                            >
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-toolbar>
        </template>
        <template v-slot:item.actions="{ item }">
            <v-btn @click="editItem(item)">
                <font-awesome-icon icon="edit" size="1x"> </font-awesome-icon>
            </v-btn>
        </template>
        <template v-slot:no-data>
            <span>Nothing to show here.</span>
        </template>
    </v-data-table>
</template>
<script lang="ts">
import { Component, Vue, Watch, Emit } from "vue-property-decorator";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import BannerFeedback from "@/models/bannerFeedback";
import Communication from "@/models/communication";
import { ResultType } from "@/constants/resulttype";
import { ICommunicationService } from "@/services/interfaces";
import { faWater } from "@fortawesome/free-solid-svg-icons";
import { ValidationProvider, extend, validate } from "vee-validate";
import { required } from "vee-validate/dist/rules";
import moment from "moment";

extend("dateValid", {
    validate(value: any, args: any) {
        if (moment(args.effective).isBefore(moment(args.expiry))) {
            return true;
        }
        return "Effective date must occur before expiry date.";
    },
    params: ["effective", "expiry"]
});

@Component({
    components: {
        ValidationProvider
    }
})
export default class CommunicationTable extends Vue {
    private communicationList: Communication[] = [];
    private communicationService!: ICommunicationService;
    private editedIndex: number = -1;
    private dialog: boolean = false;
    private isLoading: boolean = false;
    private showFeedback: boolean = false;
    private bannerFeedback: BannerFeedback = {
        type: ResultType.NONE,
        title: "",
        message: ""
    };

    private mounted() {
        this.communicationService = container.get(
            SERVICE_IDENTIFIER.CommunicationService
        );
        this.loadCommunicationList();
    }

    @Watch("dialog")
    private onDialogChange(val: any) {
        val || this.close();
    }

    private get formTitle(): string {
        return this.editedIndex === -1 ? "New Item" : "Edit Item";
    }

    private dateTimeRules(effective: Date, expiry: Date) {
        return "dateValid:" + effective.toString() + "," + expiry.toString();
    }

    private editedItem: Communication = {
        id: "-1",
        text: "",
        subject: "",
        version: 0,
        effectiveDateTime: moment(new Date()).toDate(),
        expiryDateTime: moment(new Date())
            .add(1, "days")
            .toDate()
    };

    private defaultItem: Communication = {
        id: "-1",
        text: "",
        subject: "",
        version: 0,
        effectiveDateTime: new Date(),
        expiryDateTime: moment(new Date())
            .add(1, "days")
            .toDate()
    };

    private headers: any[] = [
        {
            text: "Subject",
            value: "subject",
            align: "start",
            width: "20%",
            sortable: false
        },
        {
            text: "Effective On",
            value: "effectiveDateTime"
        },
        {
            text: "Expires On",
            value: "expiryDateTime"
        },
        {
            text: "Text",
            value: "text",
            sortable: false
        },
        {
            text: "Actions",
            value: "actions",
            sortable: false
        }
    ];

    private formatDate(date: Date): string {
        return new Date(Date.parse(date + "Z")).toLocaleString();
    }

    private dateTimeValid(): boolean {
        return moment(this.editedItem.effectiveDateTime).isBefore(
            moment(this.editedItem.expiryDateTime)
        );
    }

    private close() {
        this.dialog = false;
        this.$nextTick(() => {
            this.editedItem = Object.assign({}, this.defaultItem);
            this.editedIndex = -1;
            (this.$refs.form as Vue & {
                resetValidation: () => any;
            }).resetValidation();
        });
    }

    private save() {
        if (
            (this.$refs.form as Vue & { validate: () => boolean }).validate() &&
            this.dateTimeValid()
        ) {
            if (this.editedIndex > -1) {
                // Assign (this.editedItem) to item at this.editedIndex
                this.update(this.editedItem);
            } else {
                this.add(this.editedItem);
            }
            this.close();
        }
    }

    private editItem(item: Communication) {
        this.editedIndex = this.communicationList.indexOf(item);
        this.editedItem = item;
        this.editedItem.effectiveDateTime = new Date(item.effectiveDateTime);
        this.editedItem.expiryDateTime = new Date(item.expiryDateTime);
        this.dialog = true;
    }

    private sortCommunicationsByDate(
        isDescending: boolean,
        columnName: string
    ) {
        this.communicationList.sort((a, b) => {
            let first!: Date;
            let second!: Date;
            if (columnName === "effectiveDateTime") {
                first = a.effectiveDateTime;
                second = b.effectiveDateTime;
            } else if (columnName === "expiryDateTime") {
                first = a.expiryDateTime;
                second = b.expiryDateTime;
            } else {
                return 0;
            }

            if (first > second) {
                return isDescending ? -1 : 1;
            } else if (first < second) {
                return isDescending ? 1 : -1;
            }
            return 0;
        });
    }

    private customSort(
        items: Communication[],
        index: any[],
        isDescending: boolean[]
    ) {
        // items: 'Communication' items
        // index: Enabled sort headers value. (black arrow status).
        // isDescending: Whether enabled sort headers is desc
        if (index === undefined || index.length === 0) {
            index = ["effectiveDateTime"];
            isDescending = [true];
        }
        this.sortCommunicationsByDate(isDescending[0], index[0]);

        return this.communicationList;
    }

    private loadCommunicationList() {
        console.log("retrieving communications...");
        this.communicationService
            .getAll()
            .then((banners: Communication[]) => {
                this.communicationList = banners;
            })
            .catch((err: any) => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Error loading banners"
                };
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private add(comm: Communication): void {
        this.isLoading = true;
        this.isFinishedLoading();
        this.communicationService
            .add({
                subject: comm.subject,
                text: comm.text,
                version: 0,
                effectiveDateTime: comm.effectiveDateTime,
                expiryDateTime: comm.expiryDateTime
            })
            .then(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Success,
                    title: "Success",
                    message: "Communication Added."
                };

                this.loadCommunicationList();
            })
            .catch((err: any) => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Add communication failed, please try again"
                };
            })
            .finally(() => {
                this.isLoading = false;
                this.emitResult();
            });
    }

    private update(comm: Communication): void {
        this.isLoading = true;
        this.isFinishedLoading();
        this.communicationService
            .update({
                id: comm.id,
                subject: comm.subject,
                text: comm.text,
                version: comm.version,
                effectiveDateTime: comm.effectiveDateTime,
                expiryDateTime: comm.expiryDateTime
            })
            .then(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Success,
                    title: "Success",
                    message: "Communication Updated."
                };
                this.loadCommunicationList();
            })
            .catch((err: any) => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Error updating communication. Please try again."
                };
            })
            .finally(() => {
                this.isLoading = false;
                this.emitResult();
            });
    }

    private emitResult() {
        this.isFinishedLoading();
        this.bannerFeedbackInfo();
        this.shouldShowFeedback();
    }

    @Emit()
    private shouldShowFeedback() {
        return this.showFeedback;
    }

    @Emit()
    private bannerFeedbackInfo() {
        return this.bannerFeedback;
    }

    @Emit()
    private isFinishedLoading() {
        return this.isLoading;
    }
}
</script>