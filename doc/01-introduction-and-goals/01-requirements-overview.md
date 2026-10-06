# 1.1 Requirements Overview

## Was is Trios?

 - Trios - (**T**rading, **R**isk, **I**nformation, **O**peration, **S**ettlements)
 - Trios is a booking system to collect trading activity
 - Receives the order and trade information from different upstream systems mainly through FIX messages
 - Supports the full trade lifecycle from order routing to settlement
 - The system allocates trades for client orders
 - Generates reports, settlements, and reconciliation processes

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
