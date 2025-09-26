Feature: Get Customer
    As an operator
    I want to retrieve customer details
    So that I can view their information

    Scenario: Successfully retrieve an existing customer
        Given a customer exists in the system with ID "11111111-1111-1111-1111-111111111111"
        When I send the get customer query for ID "11111111-1111-1111-1111-111111111111"
        Then the customer details should be returned
        And no event should be saved

    Scenario: Attempt to retrieve a non-existing customer
        Given no customer exists in the system with ID "22222222-2222-2222-2222-222222222222"
        When I send the get customer query for ID "22222222-2222-2222-2222-222222222222"
        Then a NotFoundException should be thrown for get customer
        And no event should be saved
