@ui @smoke
Feature: Matching Engine Solutions navigation
  As a visitor to the Matching Engine website
  I want to explore the Solutions menu
  So that I can see what Distribution processing offers

  Scenario: Verify the Distribution processing solution content
    Given I am on the Matching Engine home page
    When I expand the Solutions menu
    Then I should see the expected list of solutions
    When I click on Distribution Processing
    Then I should be on the Distribution processing page
    When I scroll to the All-in-one solution for scale section
    Then I should see the expected All-in-one solution for scale introduction
    And I should see the expected All-in-one solution for scale content