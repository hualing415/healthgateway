declare module "*.vue" {
    import Vue from "vue";
    export default Vue;
}

declare module "*.png" {
    const value: any;
    export = value;
}

declare module "*.jpg" {
    const value: any;
    export = value;
}

declare module "*.jpeg" {
    const value: any;
    export = value;
}

declare module "idle-vue";
declare module "vuelidate";
declare module "vue-loading-overlay";