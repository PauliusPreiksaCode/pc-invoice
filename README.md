# Invoice Generation System

## Project Overview
The Invoice Generation System is designed to facilitate accurate invoicing between service providers and their clients. The system handles various scenarios related to VAT (Value Added Tax) calculation, ensuring compliance with regional tax laws. The primary function of the system is to issue correct invoices from service providers to their clients, taking into account whether VAT should be applied or not.

### Features
Client Types:
- Individual (physical person)
- Company (legal entity)
- Service Provider: Always a company (legal entity)

VAT Calculation:
- Non-VAT registered service providers do not charge VAT.
- VAT registered service providers apply VAT based on the client's location.
VAT Scenarios
- Service Provider is not VAT Registered: No VAT is charged on the order amount.
- Service Provider is VAT Registered:
  - Client Outside the EU: VAT rate is 0%.
  - Client within the EU but in a Different Country: VAT rate is the percentage applicable in the client's country. For example, in Lithuania, the VAT rate is 21%.
  - Client and Service Provider in the Same Country: The applicable VAT rate of that country is always applied.

### Main function

The main function is to generate a pdf format file with invoice data. System has some test data to try out main endpoint.

Endpoint: http://localhost:5120/Invoice/Generate

Request body example:
```
{
  "buyerId": "76afba99-366d-4ec8-b2f9-ae6459c43311",
  "sellerId": "0ef3d559-7499-4fa0-9154-507d4b2f4496",
  "itemPurchaseDtos": [
    {
      "itemId": "9bf3c81f-f728-402a-9eaf-0f4a33865bce",
      "amount": 2
    },
    {
        "itemId": "9cf0baa7-969e-4f3f-908f-4a99ddefe614",
        "amount": 3
    }, 
    {
        "itemId": "583203d8-0e17-47ec-90ec-0165a954389c",
        "amount": 5
    }
  ]
}
```