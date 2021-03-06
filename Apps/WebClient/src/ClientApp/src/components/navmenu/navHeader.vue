<script lang="ts">
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";
import { ILogger } from "@/services/interfaces";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import User from "@/models/user";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faSignInAlt, faSignOutAlt } from "@fortawesome/free-solid-svg-icons";
import RatingComponent from "@/components/modal/rating.vue";
library.add(faSignInAlt);
library.add(faSignOutAlt);

const auth = "auth";
const user = "user";
const sidebar = "sidebar";

@Component({
    components: {
        RatingComponent,
    },
})
export default class HeaderComponent extends Vue {
    @Action("toggleSidebar", { namespace: sidebar }) toggleSidebar!: () => void;
    @Getter("isOpen", { namespace: sidebar }) isOpen!: boolean;

    @Getter("oidcIsAuthenticated", {
        namespace: auth,
    })
    oidcIsAuthenticated!: boolean;

    @Getter("userIsRegistered", {
        namespace: user,
    })
    userIsRegistered!: boolean;

    @Getter("userIsActive", { namespace: user })
    userIsActive!: boolean;

    @Getter("user", { namespace: "user" }) user!: User;

    @Getter("isValidIdentityProvider", {
        namespace: auth,
    })
    isValidIdentityProvider!: boolean;

    @Ref("ratingComponent")
    readonly ratingComponent!: RatingComponent;

    private logger!: ILogger;

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
    }

    private get displayMenu(): boolean {
        return (
            this.oidcIsAuthenticated &&
            this.userIsRegistered &&
            this.userIsActive
        );
    }

    private handleToggleClick() {
        this.toggleSidebar();
    }

    private toggleMenu() {
        this.toggleSidebar();
    }

    private handleLogoutClick() {
        if (this.isValidIdentityProvider) {
            this.showRating();
        } else {
            this.processLogout();
        }
    }

    private showRating() {
        this.ratingComponent.showModal();
    }

    private processLogout() {
        this.logger.debug(`redirecting to logout view ...`);
        this.$router.push({ path: "/logout" });
    }
}
</script>

<template>
    <b-navbar toggleable="md" type="dark">
        <!-- Hamburger toggle -->
        <span
            v-if="displayMenu"
            class="navbar-toggler mr-1"
            displayMenu
            @click="handleToggleClick"
        >
            <b-icon v-if="isOpen" icon="x" class="icon-class"></b-icon>
            <b-icon v-else icon="list" class="icon-class"></b-icon>
        </span>

        <!-- Brand -->
        <b-navbar-brand class="mx-0">
            <router-link to="/timeline">
                <img
                    class="img-fluid d-none d-md-block mx-1"
                    src="@/assets/images/gov/bcid-logo-rev-en.svg"
                    width="181"
                    height="44"
                    alt="Go to healthgateway timeline"
                />

                <img
                    class="img-fluid d-md-none"
                    src="@/assets/images/gov/bcid-symbol-rev.svg"
                    width="30"
                    height="44"
                    alt="Go to healthgateway timeline"
                />
            </router-link>
        </b-navbar-brand>
        <b-navbar-brand class="px-0 pr-md-5 px-lg-5 mx-0">
            <router-link
                to="/timeline"
                class="nav-link my-0 px-0 pr-md-5 pr-lg-5 mx-0"
            >
                <h4 class="my-0 px-0 pr-md-5 pr-lg-5 mx-0">Health Gateway</h4>
            </router-link>
        </b-navbar-brand>

        <!-- Navbar links -->
        <b-navbar-nav class="ml-auto">
            <div
                v-if="oidcIsAuthenticated"
                id="menuBtnLogout"
                data-testid="logoutBtn"
                variant="link"
                class="nav-link"
                @click="handleLogoutClick()"
            >
                <font-awesome-icon icon="sign-out-alt"></font-awesome-icon>
                <span class="pl-1">Logout</span>
            </div>
            <router-link
                v-else
                id="menuBtnLogin"
                data-testid="loginBtn"
                class="nav-link"
                to="/login"
            >
                <font-awesome-icon icon="sign-in-alt"></font-awesome-icon>
                <span class="pl-1">Login</span>
            </router-link>
        </b-navbar-nav>
        <RatingComponent ref="ratingComponent" @on-close="processLogout()" />
    </b-navbar>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

.icon-class {
    width: 1.5em;
    height: 1.5em;
}

nav {
    z-index: $z_top_layer;

    a h4 {
        text-decoration: none;
        color: white;
    }
    a:hover h4 {
        text-decoration: underline;
    }
    button {
        svg {
            width: 1.5em;
            height: 1.5em;
        }
    }
    .nav-link {
        cursor: pointer;
    }
}
</style>
