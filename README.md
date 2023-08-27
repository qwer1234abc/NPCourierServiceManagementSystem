# WEB PROJECT

Background – NP Courier Service Management System
NP Courier Service (NPCS) provides express parcel delivery to more than 30 countries. In each
country, it has set up front offices to receive parcels, and a delivery station to coordinate delivery. 
The company is planning to set up a web application to improve its business productivity. The 
application should provide the following four functional areas (or packages):

1. Parcel Receiving
2. Parcel Delivery
3. Parcel Tracking
4. Administrative and Marketing Tasks

The users are:

Front Office Staff – responsible for creating a “Parcel Delivery Order” record into the system 
receiving a parcel from customers. The front office staff will also use the system to collect 
payments in cash or cash vouchers (customers to collect from a front office). Login account:
“FrontOffSG1”, password: “passFront”.

Station Manager – responsible for assigning each parcel to a delivery man. He/she will use the 
system to report the delivery status upon customers’ phone queries (parcel tracking), as well as 
responding to feedback/queries made by registered customer in the system. The station manager 
will also follow up with the parcel sender when receives a “Delivery Failure Report” in the system. 
Login account: “StationMgrSG”, password: “passStation”.

Delivery Man – will access the system for parcels to be delivered each day. He will update the 
delivery status in the system upon acknowledgement from the parcel receiver, or after custom’s 
clearance for delivery to overseas station. If delivery is unsuccessful or parcel is being damaged, 
the delivery man will have to create a “Delivery Failure Report” in the system. The login 
credentials for each delivery man is indicated in the “LoginID” and “Password” columns of the 
“Staff” table in the database of the system.

Admin Manager – responsible for updating the delivery rate and transit time for delivery to various 
destination from a delivery station. For promotion purpose, he/she can also use the system to 
issue cash voucher to a registered customer whose date of birth falls in the current month. Login 
account: “AdminMgrSG”, password: “passAdmin”.

Customers – can track the delivery status of their parcels (either as a sender or receiver). The 
customer can also give online feedback or make enquiry. For privacy protection, each customer 
can only view their feedback or enquiry, together with the corresponding responses from the 
station manager. The login credentials for each customer is indicated in the “eMailAddress” and 
“Password” columns of the “Customer” table in the database of the system.
