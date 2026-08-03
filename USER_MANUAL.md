# Addis Ababa Tourism and MICE Commission Portal

## User Manual

**System name:** Addis Ababa Tourism and MICE Commission Database Portal  
**Application:** Ministry of Tourism Portal  
**Version:** 1.0  
**Updated:** August 2026

---

## 1. Introduction

The Addis Ababa Tourism and MICE Commission Portal is a web-based system used to register, manage, review, and report on tourism establishments. It supports hotels, restaurants, MICE venues, and related tourism businesses.

The system helps establishment owners submit their business information, allows commission staff to review and approve registrations, and gives authorized users access to registry reports and operational summaries.

### Main Uses

- Register a new tourism establishment.
- Verify a registrant using OTP/SMS.
- Maintain establishment profile information.
- Upload supporting documents and certificates.
- Review pending establishment registrations.
- Approve or reject establishments.
- View and export tourism registry reports.
- Manage system users and roles.
- Submit establishment evaluation reports.

---

## 2. User Roles

The system gives each user access according to their assigned role.

| Role | Main Purpose | Main Access |
| --- | --- | --- |
| SystemAdmin | Manage users and access rights | User management dashboard |
| Commissioner | Review, approve, and report on establishments | Reports and pending approvals |
| Admin | Submit evaluation reports | Evaluation form |
| HotelOwner | Manage own establishment profile | Registry dashboard and profile forms |

If a user logs in but does not have a valid role, the system displays a no-privilege or access denied page.

---

## 3. System Requirements

### Supported Browsers

- Google Chrome
- Microsoft Edge
- Mozilla Firefox
- Safari

For the best experience, use a modern browser and a stable internet connection, especially when uploading files.

### Devices

The portal can be used on desktop, tablet, or mobile devices. Data entry and report review are easiest on a desktop or laptop screen.

---

## 4. Accessing the System

1. Open your browser.
2. Enter the portal URL provided by the system administrator.
3. The login page will appear.
4. Enter your username and password.
5. Select **Remember me** only on a trusted private device.
6. Click **Sign In**.

After login, the system opens the dashboard assigned to your role:

- HotelOwner opens the hotel owner dashboard.
- Commissioner opens the reports area.
- Admin opens the evaluation page.
- SystemAdmin opens user management.

To end your session, use **Logout** from the navigation area.

---

## 5. New Establishment Pre-Registration

New organizations can create an initial account from the login page.

### Steps

1. Click **No account? Pre-register here**.
2. Enter the organization name.
3. Enter the TIN number.
4. Enter a valid phone number.
5. Enter an email address.
6. Select the business category.
7. Send and verify the OTP when prompted.
8. Submit the registration.

After a successful registration, the system creates:

- An inactive organization record for review.
- A head office/branch record.
- An administrator user for the organization.

The default password is `admin@123`. The generated username and password are sent by SMS to the phone number entered during pre-registration.

### Validation Rules

- TIN must not already exist in the system.
- Phone number must not already be registered to another organization.
- OTP must be verified before registration can be completed.

---

## 6. OTP Verification

The portal uses OTP verification during pre-registration.

### How to Use OTP

1. Enter the phone number.
2. Click **Send OTP**.
3. Wait for the SMS verification code.
4. Enter the code in the verification field.
5. Click **Verify OTP**.
6. Continue registration after the verification succeeds.

If the OTP expires or is rejected, request a new code and try again.

---

## 7. Hotel Owner Guide

The HotelOwner role is used by establishment representatives to complete and maintain their registry profile.

### 7.1 Dashboard

The dashboard lists establishments linked to the logged-in owner. Each establishment may show:

- Trade name
- Registration name
- TIN
- Category
- Code
- Location
- Star category
- Contact details
- Current status

From the dashboard, users can view details, update registry data, and manage attachments.

### 7.2 Updating Registry Information

Registry information is organized into sections. Each section can be saved separately so users do not need to complete the entire profile at once.

Common sections include:

- Basic information
- Accommodation
- Food and beverage
- Meetings and events
- Public facilities
- Accessibility
- Safety and security
- Transport and parking
- ICT and guest services
- Utilities and resilience
- Sustainability and certifications
- Staffing and languages
- Attachments/documents

### 7.3 Basic Information

Use this section to maintain the main identity and location of the establishment:

- Trade name
- Registration name
- Specific address
- Subcity
- City and region
- Star category
- Contact information
- Reservation contact
- Location information, where available

### 7.4 Accommodation

Use this section to enter room inventory:

- King size rooms
- Twin bed rooms
- Junior suites
- Suites
- Presidential suites
- Accessible rooms
- VIP check-in availability

Total rooms and total beds are calculated from the entered room counts.

### 7.5 Food and Beverage

Use this section to enter dining and catering information:

- All-day dining availability and seats
- Specialty restaurants
- Coffee shop
- Number of bars
- Night club availability
- Souvenir shops, if applicable
- Delegation catering and maximum capacity
- Water refill stations
- Vegetarian or vegan options
- Single-use plastic reduction

For specialty restaurants, add the restaurant name, type, cuisine, capacity, halal option, vegan option, and notes.

### 7.6 Meetings and Events

Use this section to register meeting halls and event spaces.

For each room or hall, enter:

- Room name
- Type
- Width
- Length
- Ceiling height
- Capacity
- Setting arrangement

The system uses these values for MICE and event venue reports.

### 7.7 Public Facilities

Use this section to record facilities available to guests:

- Internet bandwidth
- Lobby area
- Green area
- Pool availability and type
- Spa availability and gender service
- Massage service
- Children's playground
- Child day care
- Staff canteen

### 7.8 Accessibility

Use this section to record inclusive access features:

- Wheelchair ramps
- Elevator count
- Wheelchair-sized elevators
- Public accessible bathroom

### 7.9 Safety and Security

Use this section to record safety controls:

- CCTV in public areas
- Fire extinguisher last inspection date
- Hose reels
- Smoke detectors in rooms
- Smoke detectors in public areas
- Sprinkler coverage
- Fire alarm control panel
- Emergency exits
- Bag scanner
- Walk-through scanner
- Hand scanner

### 7.10 Transport and Parking

Use this section to record transport services and parking capacity:

- Parking spaces
- Bus parking count
- Valet parking
- Parking within 100 meters
- Airport shuttle
- Public transport within 500 meters
- EV charging points
- EV charger types

### 7.11 ICT and Guest Services

Use this section to record guest service technology:

- Property-wide Wi-Fi
- Average Wi-Fi speed
- In-house laundry
- 24-hour reception
- Passport scanner
- Currency scanner

### 7.12 Utilities and Resilience

Use this section to record operational resilience:

- Standby generator capacity
- Generator coverage
- Water treatment
- Waste segregation
- Recycling
- Hazardous waste handling

### 7.13 Sustainability and Certifications

Use this section to record sustainability and compliance information:

- Sustainability focal point
- Sustainability certification
- Other eco labels
- Food waste program
- Refill water stations
- Tourism service competence license certificate
- Fire safety certificate
- Environmental clearance certificate
- Food safety and hygiene certificate
- ISO certification

### 7.14 Staffing and Languages

Use this section to enter human resource and guest communication information:

- Line staff count
- Management staff count
- International languages spoken at the front desk

Total staff is calculated from line staff and management staff.

### 7.15 Saving a Section

1. Open the establishment profile.
2. Complete or update the required section.
3. Click the section's **Save** button.
4. Wait for the success message before leaving the page.

If a save fails, check your internet connection and try again. For large profiles, save one section at a time.

---

## 8. Attachments and Documents

HotelOwner users can upload supporting documents for an establishment.

### Common Attachment Categories

| Category | Examples |
| --- | --- |
| Business License | Trade license or registration certificate |
| Tax Certificate | TIN certificate or tax clearance |
| Property Photos | Exterior, interior, rooms, halls |
| Health and Safety Certificate | Hygiene or inspection documents |
| Fire Safety Certificate | Fire inspection certificate |
| Environmental Permit | Environmental clearance |
| Other | Any additional supporting document |

### Uploading a File

1. Open the establishment profile.
2. Go to the attachments/documents area.
3. Select the document category.
4. Choose the file from your device.
5. Click **Upload**.
6. Wait for the confirmation message.

Supported file types include common document and image formats such as PDF, JPG, PNG, GIF, and WebP.

### Viewing or Deleting Files

- To view a file, click the file link in the attachment list.
- To delete a file, use the delete action beside the attachment.
- Deleted files are removed from the system record and the file storage location when possible.

---

## 9. Commissioner Guide

The Commissioner role is used to review registrations, manage approvals, and access reports.

### 9.1 Reports Center

After login, Commissioner users are taken to the reports center. The reports center contains categories such as:

- Registry
- MICE
- Technology
- Compliance

Each category contains report tabs. The available reports include:

- Hotel Registry
- Room Inventory
- Bed Capacity
- Star Rating
- Certified Facilities
- Parking Capacity
- Meeting and Conference Facilities
- Conference Hall Capacity
- MICE-Ready Facilities
- MICE Destinations
- Event Venues
- Technology Systems
- Kitchen and POS
- Specialized Restaurants
- Seating Capacity
- Bars and Lounge
- Health and Safety
- Accessibility
- Sustainability
- PPP Analytics

### 9.2 Searching Reports

1. Open the reports center.
2. Select a category.
3. Select a report tab.
4. Type in the search box.
5. The report table filters to matching rows.

### 9.3 Exporting Reports

1. Open the desired report.
2. Use the search box if you only want filtered rows.
3. Click **Export**.
4. The system downloads the visible report data as a CSV file.

### 9.4 Printing Reports

1. Open the desired report.
2. Click **Print**.
3. Use the browser print dialog to choose printer, paper size, and layout.

### 9.5 Pending Approvals

The pending approvals page lists organizations that require review. Each item may show:

- Property name
- TIN
- Code
- Subcity
- Specific address
- Star rating
- Active/inactive status
- Preference/category

### 9.6 Approving an Establishment

1. Open **Pending Approvals**.
2. Review the establishment information.
3. Open details if more information is needed.
4. Click **Approve**.
5. Confirm the action.

After approval, the establishment is activated and appears as approved in reports and registry views.

### 9.7 Rejecting an Establishment

1. Open **Pending Approvals**.
2. Review the establishment information.
3. Click **Reject**.
4. Enter a rejection comment when needed.
5. Confirm the action.

After rejection, the establishment remains inactive. The owner should correct the submitted information or contact the commission office for follow-up.

---

## 10. Admin Evaluation Guide

The Admin role is used to submit establishment evaluations.

### Steps

1. Log in as an Admin user.
2. Open the evaluation page.
3. Enter or confirm the establishment name.
4. Select the evaluation date.
5. Complete the evaluation categories and ratings.
6. Click **Submit**.
7. Wait for the success message.

The current evaluation page uses predefined evaluation categories.

---

## 11. System Admin Guide

The SystemAdmin role is used to create, update, activate, deactivate, and delete users.

### 11.1 User Management Dashboard

The dashboard lists users with information such as:

- First name
- Second name
- Username
- Phone number
- Role
- Active status
- Edit/delete actions

### 11.2 Creating a User

1. Open the SystemAdmin dashboard.
2. Choose the option to create or add a user.
3. Enter first name and second name.
4. Enter a unique username.
5. Enter phone number.
6. Enter a password, or leave it blank to use the default password `admin@123`.
7. Select the role.
8. Set the user as active if they should log in immediately.
9. Save the user.

### 11.3 Editing a User

1. Open the SystemAdmin dashboard.
2. Select the user to edit.
3. Update the name, username, phone, role, password, or active status.
4. Leave the password blank if it should not be changed.
5. Save the changes.

### 11.4 Deactivating a User

1. Edit the user.
2. Turn off or uncheck the active status.
3. Save the user.

The user remains in the system but cannot use the account normally while inactive.

### 11.5 Deleting a User

1. Open the SystemAdmin dashboard.
2. Select **Delete** for the user.
3. Confirm the action.

Delete users only when the account is no longer needed. For temporary access removal, deactivate the user instead.

### 11.6 Role Mapping

| Display Role | System Role | Typical User |
| --- | --- | --- |
| System Administrator | SystemAdmin | System administrator |
| Hotel Administrator | HotelOwner | Establishment owner or representative |
| Supervisor | Admin | Evaluation officer |
| General Manager | Commissioner | Commissioner or authorized reviewer |

---

## 12. Recommended Operating Procedures

### For Hotel Owners

- Complete the basic information section first.
- Save each section after entering data.
- Upload required licenses and certificates before review.
- Keep room, meeting, parking, and certification data accurate.
- Contact the commission office if the registration is rejected.

### For Commissioners

- Review all establishment details before approval.
- Use rejection comments to explain required corrections.
- Use reports to monitor registry completeness and facility capacity.
- Export reports when sharing data outside the portal.

### For System Administrators

- Assign the minimum role required for each user.
- Deactivate users who should no longer access the system.
- Avoid sharing default passwords.
- Encourage users to change default passwords after first login.

---

## 13. Troubleshooting

| Problem | Likely Cause | What to Do |
| --- | --- | --- |
| Login fails | Wrong username or password | Check credentials or contact SystemAdmin |
| User opens no-privilege page | User has no valid role | Ask SystemAdmin to assign a role |
| OTP not received | Phone issue, SMS delay, or service issue | Wait briefly, verify the number, and request another OTP |
| OTP rejected | Code expired or entered incorrectly | Request a new OTP |
| TIN already exists | Organization was previously registered | Contact support or use existing credentials |
| Phone already exists | Phone number is linked to another organization | Use another number or contact support |
| Registry section does not save | Network/API issue or invalid data | Review fields, save again, or try one section at a time |
| Attachment upload fails | File issue or storage connection issue | Check file type/size and retry |
| Report data looks incomplete | Establishment profile is incomplete | Ask the owner to update missing profile sections |
| Export does not download | Browser blocked download | Allow downloads and try again |

---

## 14. Glossary

| Term | Meaning |
| --- | --- |
| AATMCDP | Addis Ababa Tourism and MICE Commission Database Portal |
| Attachment | Uploaded document or image linked to an establishment |
| Commissioner | User who reviews registrations and reports |
| Consignee | Organization or person record in the connected CNET system |
| Establishment | Tourism business registered in the portal |
| HotelOwner | User who manages an establishment profile |
| MICE | Meetings, Incentives, Conferences, and Exhibitions |
| OTP | One-Time Password sent by SMS for verification |
| PPP | Public-Private Partnership |
| Registry | The official profile record of an establishment |
| SystemAdmin | User who manages user accounts and roles |
| TIN | Tax Identification Number |

---

## 15. Support

For account access, password reset, role assignment, or technical issues, contact the System Administrator.

For registration status, approval questions, or rejected applications, contact the Commissioner office or the assigned review officer.

For document requirements and operational policy questions, contact the Addis Ababa Tourism and MICE Commission.

