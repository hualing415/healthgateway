﻿<script lang="ts">
import Vue from "vue";
import { Component } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";
import type { WebClientConfiguration } from "@/models/configData";
const namespace = "auth";

@Component
export default class LogoutView extends Vue {
    @Action("signOutOidc", { namespace }) logout!: () => void;

    @Getter("oidcIsAuthenticated", { namespace }) oidcIsAuthenticated!: boolean;
    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;

    private mounted() {
        if (this.oidcIsAuthenticated) {
            this.logout();
        }
    }

    private created() {
        setTimeout(() => {
            if (this.$route.path == "/logout") {
                this.$router.push({ path: "/" });
            }
        }, Number(this.config.timeouts.logoutRedirect));
    }
}
</script>

<template>
    <div class="row justify-content-center h-100 pt-5">
        <div class="col-lg-6 col-md-6 pt-2">
            <div
                v-if="!oidcIsAuthenticated"
                class="shadow-lg p-3 mb-5 bg-white rounded border"
            >
                <h3>
                    <strong>You signed out of your account</strong>
                </h3>
                <p>It's a good idea to close all browser windows.</p>
            </div>
            <div v-else>
                <h3>Signing out...</h3>
            </div>
        </div>
    </div>
</template>
