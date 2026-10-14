# 1.1 Requirements Overview

## What is Template?

 - Template is a starting point for modular monolith solutions (Clean Architecture, Vertical Slices)
 - Describe the purpose of your system here
 - List the main capabilities and upstream/downstream systems

## Main features

- **FR1. Change Logging**  
  System records all data and configuration changes (who/what/when/before/after).
- **FR2. Original Data Storage**  
  System stores immutable original input data and the processed data should link it.
- **FR3. User Error Handling**  
  Users can view, correct, and resubmit failed transactions.
- **FR4. Business Traceability**  
  System provides end-to-end tracking of each business case with status visibility.
- **FR5. Business Control Actions**  
  Authorized users can pause, retry, cancel, or escalate cases.
- **FR6. Client-Specific Workflows**  
  System supports configurable workflows per client.
- **FR7. Configuration Management**  
  Business rules and client parameters are managed via configuration (not code).
