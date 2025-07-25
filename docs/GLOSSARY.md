# OPERATION PRIME Glossary

**Keywords**: definitions, terminology, technical terms, incident management, NOC operations

## Core Terms

### **Audit Trail**
Complete record of all changes made to an incident, including timestamps, user actions, and field modifications for compliance purposes.

### **Business Impact**
Description of how an incident affects business operations, revenue, or customer experience.

### **Circuit Breaker**
Design pattern that prevents cascading failures by stopping requests to a failing service after a threshold is reached.

### **ContentDialog**
WinUI 3 modal dialog component used for wizards and detailed forms in OPERATION PRIME.

### **Conversion**
Process of escalating a Pre-Incident to Major Incident status when severity increases.

### **EF Core (Entity Framework Core)**
Microsoft's object-relational mapping (ORM) framework used for database operations.

### **Impacted Users**
Number of users affected by an incident, used in priority calculation matrix.

### **Major Incident**
Critical incident requiring comprehensive documentation, NOI generation, and formal escalation procedures.

### **MVVM (Model-View-ViewModel)**
Architectural pattern separating user interface from business logic using data binding.

### **Neurons**
External incident management system that OPERATION PRIME integrates with for data synchronization.

### **NOC (Network Operations Center)**
Centralized location for monitoring and managing network infrastructure and incidents.

### **NOI (Notice of Incident)**
Formal document generated for Major Incidents to notify stakeholders of critical issues.

### **Offline-First**
Design approach where the application works fully without internet connectivity, syncing when available.

### **Pre-Incident**
Minor incident with streamlined workflow, focusing on quick resolution with optional escalation.

### **Priority Matrix**
Automated calculation system using Urgency × Impacted Users to determine incident priority (P1-P4).

### **SQLCipher**
Encrypted SQLite database extension providing AES-256 encryption for data at rest.

### **Urgency**
Assessment of how quickly an incident needs resolution: High (1), Medium (2), Low (3).

### **WAL Mode (Write-Ahead Logging)**
SQLite journal mode that allows concurrent reads while writing, improving performance.

### **WinUI 3**
Microsoft's modern UI framework for Windows applications, successor to UWP and WinForms.

## Technical Acronyms

### **AES-256**
Advanced Encryption Standard with 256-bit key length, used for data encryption.


### **CRUD**
Create, Read, Update, Delete - basic database operations.

### **HTTP**
HyperText Transfer Protocol - standard for web communication.

### **JSON**
JavaScript Object Notation - lightweight data interchange format.

### **ORM**
Object-Relational Mapping - technique for database interaction using objects.

### **REST**
Representational State Transfer - architectural style for web services.

### **SQL**
Structured Query Language - standard language for database operations.

### **UI/UX**
User Interface / User Experience - design and usability aspects.

### **XAML**
eXtensible Application Markup Language - Microsoft's markup language for UI definition.

## Status Values

### **New**
Incident just created, no action taken yet.

### **In Progress**
Incident actively being worked on by NOC staff.

### **Resolved**
Issue has been fixed but awaiting final closure confirmation.

### **Closed**
Incident completed with all documentation and follow-up actions finished.

## Priority Levels

### **P1 (Priority 1)**
Critical incidents requiring immediate attention, typically affecting 200+ users or high urgency.

### **P2 (Priority 2)**
High priority incidents needing prompt resolution within defined SLA.

### **P3 (Priority 3)**
Medium priority incidents with standard resolution timeframes.

### **P4 (Priority 4)**
Low priority incidents that can be addressed during normal business hours.

## Source Types

### **Service Desk**
Incidents reported through the IT service desk system.

### **NOC**
Incidents detected directly by Network Operations Center monitoring.

### **SME (Subject Matter Expert)**
Incidents reported by technical specialists or system experts.

### **Business Escalation**
Incidents escalated from business units due to operational impact.

---
*This glossary covers technical terms used throughout OPERATION PRIME documentation and the application interface.*
