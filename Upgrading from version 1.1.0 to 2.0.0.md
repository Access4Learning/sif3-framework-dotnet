Upgrading from version 1.1.0 to 2.0.0
--------------------------------------

Version 2.0.0 of the SIF3 Framework includes modifications to the database schema to fix a bug with handling a default zone for an environment. The fix involves changing some primary keys from a GUID to an integer. Such a change is non-trivial, and the provided update SQL script (Scripts\SQL\Update database schema 1.1.0 to 2.0.0.sql) does not contain all statements required to perform the change. Instead, the following link outlines a proposed approach to update the database properly.

> [http://stackoverflow.com/questions/2730305/approach-for-altering-primary-key-from-guid-to-bigint-in-sql-server-related-tabl](http://stackoverflow.com/questions/2730305/approach-for-altering-primary-key-from-guid-to-bigint-in-sql-server-related-tabl "Approach for altering Primary Key from GUID to BigInt in SQL Server related tables")

As the bug prevented the use of a default zone, it is unlikely that an existing database would have appropriate entries for the affected database tables. As such, it may be that the approach outlined in the article is simplified.
