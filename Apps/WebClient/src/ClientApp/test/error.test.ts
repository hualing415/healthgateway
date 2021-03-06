import { mount, shallowMount } from "@vue/test-utils";
import NotFoundView from "@/views/errors/notFound.vue";
import UnauthorizedView from "@/views/errors/unauthorized.vue";
import ErrorComponent from "@/components/error.vue";
import PageError from "@/models/pageError";
import container from "@/plugins/inversify.config";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { ILogger } from "@/services/interfaces";

describe("ErrorComponent", () => {
    const logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);
    logger.initialize("info");
    test("renders error component properties correctly", () => {
        const error: PageError = new PageError(
            "777",
            "ERROR_NAME",
            "ERROR_MESSAGE"
        );
        const wrapper = shallowMount(ErrorComponent, { propsData: { error } });

        expect(wrapper.find("h1").text()).toBe(error.code);
        expect(wrapper.find("h2").text()).toBe(error.name);
        expect(wrapper.find("p").text()).toBe(error.message);
    });
});

describe("NotFoundView", () => {
    test("renders notfound properties correctly", () => {
        const mountWrapper = mount(NotFoundView);
        const errorDescription = mountWrapper.vm.$data.errorDescription;
        expect(mountWrapper.find("h1").text()).toBe(errorDescription.code);
        expect(mountWrapper.find("h2").text()).toBe(errorDescription.name);
        expect(mountWrapper.find("p").text()).toBe(errorDescription.message);
    });
});

describe("UnauthorizedView", () => {
    test("renders unauthorized properties correctly", () => {
        const mountWrapper = mount(UnauthorizedView);
        const errorDescription = mountWrapper.vm.$data.errorDescription;
        expect(mountWrapper.find("h1").text()).toBe(errorDescription.code);
        expect(mountWrapper.find("h2").text()).toBe(errorDescription.name);
        expect(mountWrapper.find("p").text()).toBe(errorDescription.message);
    });
});
