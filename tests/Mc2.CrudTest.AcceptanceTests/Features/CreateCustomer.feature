Feature: Create Customer

As an operator, I want to create a new customer so that I can manage their information.

Scenario: Create a customer with valid details

Given I have a valid customer creation request

When I send the create customer command

Then the customer should be created successfully

And a CustomerCreatedEvent should be raised

Scenario: Attempt to create a customer with invalid phone number

Given I have a customer creation request with an invalid phone number

When I send the create customer command

Then an InvalidPhoneNumberException should be thrown

Scenario: Attempt to create a customer with duplicate email

Given I have a customer creation request with a duplicate email

When I send the create customer command

Then a DuplicateEmailException should be thrown

Scenario: Attempt to create a customer with duplicate personal info

Given I have a customer creation request with duplicate name and date of birth

When I send the create customer command

Then a DuplicateCustomerException should be thrown