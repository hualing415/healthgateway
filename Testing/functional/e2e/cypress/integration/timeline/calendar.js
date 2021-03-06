const { AuthMethod } = require("../../support/constants")

describe('Calendar View', () => {
    beforeEach(() => {
        cy.readConfig().as("config").then(config => {
            config.webClient.modules.CovidLabResults = false
            config.webClient.modules.Comment = false
            config.webClient.modules.Encounter = false
            config.webClient.modules.Immunization = false
            config.webClient.modules.Laboratory = false
            config.webClient.modules.Medication = true
            config.webClient.modules.MedicationHistory = false
            config.webClient.modules.Note = false
            cy.server();
            cy.route('GET', '/v1/api/configuration/', config);
        })
        cy.login(Cypress.env('keycloak.username'), 
                 Cypress.env('keycloak.password'), 
                 AuthMethod.KeyCloak);
        cy.checkTimelineHasLoaded();
    })

    it('Navigate to Month View', () => {
        cy.get('[data-testid=monthViewToggle]')
            .first()
            .click();
        cy.get('#currentDate')
            .should('be.visible');
        cy.location('hash')
            .should('eq', '#calendar');
    })

    it('Validate Year Selector', () => {
        cy.get('[data-testid=monthViewToggle]')
            .first()
            .click();
        cy.get('#currentDate').click();
        cy.get('#currentDate').click();
        cy.get('.years')
            .children()
            .should('be.visible');
        cy.get('[data-testid=yearBtn2019]').click();
        cy.get('[data-testid=monthBtnJan]').click();
        cy.get('#currentDate')
            .should('not.have.text', ' January 2019 ');
        cy.get('[data-testid=monthBtnFeb]').click();
        cy.get('#currentDate')
            .should('have.text', ' February 2019 ');
    })
})