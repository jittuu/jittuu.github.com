---
layout: post
title: "Multitenancy - Data"
---

When dealing with data in multi-tenancy application, we have think about physical storage and extensibility.

# Physical storage

Regarding storing data in RDBMS, especially in SQL Server, we have a few options.

1. Shared database instance, i.e. add tenantId to all the the tables related to tenants and query by tenantId and other filters.
2. Multiple database instances, i.e. create each database for each tenant.

The first option looks simpler compare to the second one, but if we think about security, isolation, backup and _horizontal scaling_, it brings more challenges.

 - **Security** - I believe multiple db instances option is much more simple than shared database option when it comes to security.
 - **Isolation** - Obviously, the multiple db instances is better.
 - **Backup** - It is also much easier to work with multiple instances to backup, extract data and give it to your tanent when it is requested. And the cost is also much more predictable since it is just related to numbers of tenants you have.
 - **Scaling** - I think, this is the major issue of shared database instance. The option to scale horizontally in Sql Server is through [partitioning][]. It is way more complex than putting multiple db instances on multiple servers. And it also have flexibility to make more pricing options such as dedicated db server for one tenant.
 - **Release Management** - This is one advantage of shared database instance. It is much easier to add one more record in database than creating new database, setting up security option, etc. Of curse, we still can automate this but it is more complex than shared db instance.
 
# Extensibility

When tenants want to customize _behaviour_ of the system, they usually require some _custom data_ to work with. We also have some patterns to work with:

 - **Preallocated Fields** - such as Field1, Field2, Field3, etc.It will usually lead to _maintenace nightmare_ because the developer have no idea what is the usage of Field1. Field1 can be different usages for different tenants as well.
 - **Name-Value Pairs** - with one value table (TenantId, ExtId, Value) and one meta table (TenantId, ExtId, ValueDesc, ValueDataType). The main dis-advantage is query. It is very complicated to make efficient query with those kind of tables. The performance is usually the issue when data grow.
 - **Custom Columns** - directly add more columns to existing table. It is the simplest solution to data extensibility. The only drawback is conflicts between customer-defined-column and newly-added-column. It can happen when we want to add new column in next version but customer alread use that column. E.g. After we release v1, tenant A add "Location" column to customer table. When we want to add "Location" column in v2, we have conflict with tenant A.
 - **xxxxx_Extented** - add one-to-one relationship extended table to existing table. By doing this, we still have simplicity to work with custom data since we just need a single join, as well as solving the versioning issue. E.g. Cusomter table and Customer_Extended table with additional tenant-added-columns.
 
In next post, I'll talk about application, especially web app.

[partitioning]: http://technet.microsoft.com/en-us/library/ms188232(v=sql.105).aspx
