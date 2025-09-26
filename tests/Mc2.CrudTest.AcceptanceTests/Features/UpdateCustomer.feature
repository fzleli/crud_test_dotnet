Feature: Update Customer

As an operator, I want to update an existing customer so that I can modify their information.

Scenario: Update a customer with valid details
    Given I have an existing customer with valid data
    And I have a valid update request
    When I send the update customer command
    Then the customer should be updated successfully
    And a CustomerUpdatedEvent should be raised

Scenario: Attempt to update a customer with invalid phone number
    Given I have an existing customer with valid data
    And I have an update request with an invalid phone number
    When I send the update customer command
    Then an InvalidPhoneNumberException should be thrown while updating a customer

Scenario: Attempt to update a customer with duplicate email
    Given I have an existing customer with valid data
    And I have an update request with a duplicate email
    When I send the update customer command
    Then a DuplicateEmailException should be thrown while updating a customer

Scenario: Attempt to update a customer with duplicate personal info
    Given I have an existing customer with valid data
    And I have an update request with duplicate name and date of birth
    When I send the update customer command
    Then a DuplicateCustomerException should be thrown while updating a customer

Scenario: Attempt to update a customer that does not exist
    Given I have an update request for a non-existent customer
    When I send the update customer command
    Then a NotFoundException should be thrown
