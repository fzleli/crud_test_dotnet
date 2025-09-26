Feature: Delete Customer
    As an operator
    I want to delete an existing customer
    So that I can remove their information from the system

    Scenario: Successfully delete an existing customer
        Given a customer exists in the system with ID "11111111-1111-1111-1111-111111111111"
        When I send the delete customer command for ID "11111111-1111-1111-1111-111111111111"
        Then the customer should be removed successfully
        And a CustomerDeletedEvent should be raised

    Scenario: Attempt to delete a non-existing customer
        Given no customer exists in the system with ID "22222222-2222-2222-2222-222222222222"
        When I send the delete customer command for ID "22222222-2222-2222-2222-222222222222"
        Then a NotFoundException should be thrown

    Scenario: Attempt to delete a customer already deleted
        Given a customer exists in the system with ID "33333333-3333-3333-3333-333333333333" and is already deleted
        When I send the delete customer command for ID "33333333-3333-3333-3333-333333333333"
        Then a NotFoundException should be thrown
