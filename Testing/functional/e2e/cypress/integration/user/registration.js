const { AuthMethod } = require("../../support/constants")

describe('Registration', () => {
    beforeEach(() => {
        cy.login(Cypress.env('keycloak.username'), 
                 Cypress.env('keycloak.password'), 
                 AuthMethod.KeyCloak)
    })

    it('Test1', () => {

    })

    it('Test2', () => {

    })
})