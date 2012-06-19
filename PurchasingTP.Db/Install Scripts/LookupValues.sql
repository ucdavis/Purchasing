﻿insert into OrderStatusCodes (Id,Name,Level,IsComplete,KfsStatus,ShowInFilterList) values ('AM','Account Manager',3,0,0,1),('AP','Approver',2,0,0,1),('CA','Conditional Approval',2,0,0,0),('CN','Complete-Not Uploaded KFS',5,1,0,0),('CP','Complete',5,1,0,1),('OC','Cancelled',5,1,0,1),('OD','Denied',5,1,0,1),('PR','Purchaser',4,0,0,1),('RQ','Requester',1,0,0,0)
insert into Roles (Id,Name,Level,IsAdmin) values ('AD','Admin',0,1),('DA','Departmental Admin',0,1),('RQ','Requester',1,0),('AP','Approver',2,0),('AM','Account Manager',3,0),('PR','Purchaser',4,0),('EU','Emulation User',0,1),('RV','Reviewer',0,0)
insert into ShippingTypes (Id,Name,Warning) values ('EX','Expedited',''),('ON','Overnight','This shipping may cost a lot of money.'),('ST','Standard',''),('WC','Will Call','')
insert into States (Id,Name) values ('AK','ALASKA '),('AL','ALABAMA'),('AR','ARKANSAS'),('AZ','ARIZONA'),('CA','CALIFORNIA'),('CO','COLORADO'),('CT','CONNECTICUT'),('DC','DISTRICT OF COLUMBIA'),('DE','DELAWARE'),('FL','FLORIDA'),('GA','GEORGIA'),('HI','HAWAII '),('IA','IOWA'),('ID','IDAHO'),('IL','ILLINOIS'),('IN','INDIANA'),('KS','KANSAS '),('KY','KENTUCKY'),('LA','LOUISIANA'),('MA','MASSACHUSETTS'),('MD','MARYLAND'),('ME','MAINE'),('MI','MICHIGAN'),('MN','MINNESOTA'),('MO','MISSOURI'),('MS','MISSISSIPPI'),('MT','MONTANA'),('NC','NORTH CAROLINA'),('ND','NORTH DAKOTA'),('NE','NEBRASKA'),('NH','NEW HAMPSHIRE'),('NJ','NEW JERSEY'),('NM','NEW MEXICO'),('NV','NEVADA '),('NY','NEW YORK'),('OH','OHIO'),('OK','OKLAHOMA'),('OR','OREGON '),('PA','PENNSYLVANIA'),('PR','PUERTO RICO'),('RI','RHODE ISLAND'),('SC','SOUTH CAROLINA'),('SD','SOUTH DAKOTA'),('TN','TENNESSEE'),('TX','TEXAS'),('UT','UTAH'),('VA','VIRGINIA'),('VI','U.S. VIRGIN ISLANDS'),('VT','VERMONT'),('WA','WASHINGTON'),('WI','WISCONSIN'),('WV','WEST VIRGINIA'),('WY','WYOMING')
insert into OrderTypes (Id,Name,PurchaserAssignable) values ('CS ','Campus Services',1),('DPO','Departmental Purchase Order',0),('DRO','Departmental Repair Order',0),('KFS','KFS Document',1),('MT ','My Travel',1),('OR ','Order Request',0),('PC ','Purchasing Card Order',1),('PO ','Campus Purchase Order',0),('PR ','Purchase Request',0)