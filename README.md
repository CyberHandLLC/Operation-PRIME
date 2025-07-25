# OPERATION PRIME - Operations Primary ResponseIncident Management System

**Keywords**: incident management, NOC, network operations, pre-incident, major incident, priority matrix, NOI, Neurons integration, offline-first, WinUI3, MVVM

**Version**: 1.0  
**Date**: July 24, 2025  
**Status**: Development Phase

## Table of Contents

1. [Clean Architecture & Foundation](#clean-architecture--foundation)
2. [Eight Core Implementation Principles](#eight-core-implementation-principles)
3. [Overview](#overview)
4. [Key Features](#key-features)
5. [Technology Stack](#technology-stack)
6. [Quick Start](#quick-start)
7. [Documentation Structure](#documentation-structure)
   - [Core Documentation](#core-documentation)
   - [Reference Materials](#reference-materials)
8. [Security & Compliance](#security--compliance)
9. [Support & Contact](#support--contact)

## Clean Architecture & Foundation

OPERATION PRIME is built on a strict Clean Architecture foundation:
- **Domain Layer**: Pure C# models, business rules, value objects
- **Application Layer**: Use-cases, workflow logic, repository/service interfaces
- **Infrastructure Layer**: EF Core, SQLCipher, repository/service implementations
- **Presentation Layer**: WinUI 3 UI, ViewModels, XAML, Services, Utils

All dependencies flow inward, and the project enforces DI/service registration, audit logging, and error handling from the start.

## Eight Core Implementation Principles

OPERATION PRIME is built according to these 8 core architectural principles:

1. **Four Core Layers**: Domain, Application, Infrastructure, Presentation with strict separation and inward dependency flow
2. **Dependency Injection & Service Registration**: Microsoft.Extensions.DependencyInjection with proper service lifetimes for all components
3. **Configuration & Options Pattern**: appsettings.json + IOptions<T> for all configuration management
4. **Testing & Architecture Validation**: Domain.Tests, Application.Tests, and NetArchTest for boundary enforcement
5. **Logging & Error Handling**: ILogger<T> integration across all layers with structured audit logging
6. **Web Request Wrapper Service**: Abstracted external service calls (Neurons integration) behind interfaces
7. **Validation & DTOs**: Toolkit validation evolving to FluentValidation, with DTOs for structured workflows
8. **Data Flow Architecture**: Clear separation between UI, business logic, and data access with proper abstraction

> **📋 All documentation, code, and implementation follows these 8 principles as the foundation for maintainable, testable, and scalable architecture.**

## Overview

OPERATION PRIME is a secure, offline-first desktop application for managing network incidents in enterprise environments. It handles two types of incidents:
- **Pre-Incidents**: Minor issues requiring quick resolution
- **Major Incidents**: Critical events requiring comprehensive documentation and NOI (Notice of Incident) generation

## Key Features

- **Clean Architecture Foundation**: Strict four-layer separation (Domain, Application, Infrastructure, Presentation) for maintainability and testability
- **Dependency Injection & Service Registration**: All services, repositories, and ViewModels are registered with correct lifetimes using Microsoft.Extensions.DependencyInjection
- **Early Audit Logging & Error Handling**: Audit trail and ILogger<T> integration are enforced from the start for compliance and diagnostics
- **Offline-First Architecture**: Works without internet connectivity, syncs when available
- **Dual Incident Types**: Streamlined Pre-Incident flow with optional escalation to Major
- **Priority Matrix**: Automated priority calculation based on urgency × impacted users
- **NOI Generation**: Automated Notice of Incident creation and distribution with email notifications
- **Audit Trail**: Complete change tracking and compliance logging
- **Neurons Integration**: Sync connection to Neurons by allowing user to login to get a token to use to fetch incident data
- **Multi-User Support**: Supports up to 5-10 concurrent users
- **Data Encryption**: AES-256 encryption with SQLCipher for local data security

## Technology Stack

- **Framework**: .NET 9 with WinUI 3
- **Architecture**: MVVM (Model-View-ViewModel)
- **Database**: SQLite with Entity Framework Core
- **Security**: SQLCipher for encryption
- **Neurons Integration**: HttpClient for Neurons HTTP calls connectivity
- **Language**: C# with XAML for UI

## Quick Start

1. **Installation**: Run the MSI installer (no admin rights required)
2. **First Launch**: Configure Neurons connection by login, fetches user details and token
3. **Create Incident**: Use the Dashboard wizard to create Pre or Major incidents
4. **Manage**: View, edit, and track incidents through the main interface

## Documentation Structure

### Core Documentation
- [Quick Reference](./docs/QUICK_REFERENCE.md) - Cheat sheet for common tasks and workflows
- [Architecture Guide](./docs/ARCHITECTURE.md) - Technical architecture and design patterns
- [User Interface Guide](./docs/UI_GUIDE.md) - UI components and design specifications
- [Workflow Guide](./docs/WORKFLOWS.md) - Detailed incident management workflows

### Reference Materials
- [Reference](./docs/REFERENCE.md) - Field definitions and data structures
- [Glossary](./docs/GLOSSARY.md) - Technical terms and definitions
- [Coding Standards](./docs/CODING_STANDARDS.md) - Clean code practices and best practices
- [Development Guide](./docs/DEVELOPMENT.md) - Development phases and implementation details
- [Technical Specifications](./docs/TECHNICAL_SPECS.md) - Complex system implementations

## Security & Compliance

- **Data Encryption**: All local data encrypted with AES-256
- **Audit Logging**: Complete action tracking for compliance
- **No Hardcoded Keys**: Environment-based configuration
- **Input Validation**: Comprehensive sanitization and validation

## Support & Contact

For technical support or questions about OPERATION PRIME, contact the development team.

---
*This documentation is part of the OPERATION PRIME project and is subject to regular updates during development.*
