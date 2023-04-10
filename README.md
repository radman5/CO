# RadPay Demo Payment Gateway

 - The Payment API is a .NET 7 service
 - The Payment Db while in development is created on application start using EF Core code first.
   - The database is SQLite, will be migrated to Postgres once development has been completed
 - The integration tests spin up the API on a test web server and calls to it using an HttpClient.

 ### Entities explained
 
 - There a three entities in the system
   - MerchantPaymentProfile
      - This entity stores a reference to its account in the bank system
      - Also has a reference to the merchants internal account id that is added as a header in the api gateway.
         - This header is currently used to link the other resources to a merchant and as a second authorisation layer  ensuring a merchant only has access to resources it created 
   - CardDetails
      - This entity stores the credit card details (currently as plain text, encrypted in the future) and a randomly generated string called token.
         - The token is passed back to the requesting merchant server and is used to reference the card details when making a payembt request
      - CardDetails are currently single use ensuring you cannot accidentally make duplicate payment requests
   - Payment
      - This entity stores payment details and payment token
      - Can be retrieved by the merchant 


### Prerequisites 
1. Visual Studio 2022 (or any with .NET 7 support) 
2. Docker Desktop installed and running
## How to run solution

 1. Ensure debug profile in Visual Studio is set to the "Docker" profile
 2. Click the "Docker" play button
 3. Once the application is running, the OpenApi docs page will open in your default browser.
    1. You can either use this, Postman or other api testing tools.

## How to make a Successful payment

Make sure to follow the OpenApi docs, it has the contracts and examples ready to use

1. First you need a valid MerchantId to send as a header
   1. There is a merchant created for testing with MerchantId = 1
   2. Header: Key = "MerchantId", Value = "1"
2. Then you need to request a payment token
   1. Request payment token using your credit card details
   2. Dont forget to send a "MerchantId" header
3. Once you have a payment token you are ready make a payment
   1. To request a payment you need to send:
      1. your payment token (single use)
      2. amount eg 100
      3. currencty eg "GBP"
4. Payment will always be successful because you are rich!

## How to make a failed payment

To trigger a failed payment, request a payment token using CardNumber = "insuffientfunds"

## How to run tests
You can run the tests using the built in Visual Studio Test Explorer/Runner

## Assumptions

 - There is an API Gateway component/system in between the user and the payment gateway that handles things like:
	 - Merchant authentication and authorisation
	 - Api usage
	 - Rate limiting
	 - General security measures
		 - DDoS etc.
 - Card details cannot be saved to be used in the future (single use)

## Areas for improvement
 - Asymmetric encryption of the credit card details
 - Fraud detection service checking customer, merchant and payment details that can block or flag card details and or payments made
 - Ability to save card details in a way that they can be reused without having to re-enter them after every purchase
 - Add logging and metrics that publish to a service like New Relic of Datadog to monitor the service once deployed
 - Caching is always a powerful tool that could be added in places, probably in the API Gateway component but I think most traffic on this service would be writes so caching would not be as useful in other read heavier components.
 - Move PaymentDb definition to another repo.
   - Will need to publish the paymentDb as a docker container to a shared image repository like artifactory to be consumed by paymentDb integration tests.
 - Add batect to manage a local dev environment with depencies like PaymentDb
 
## Architecture
###  Cloud infrastructure considerations
 - NLB in front of the payment system
	 - Faster than ALB
	 - Can handle traffic spikes better which would be useful during times like Christmas and Black Friday sales
 - All APIs and Worker applications are .NET 7 applications deployed as separate services on a single kubernetes cluster.
	 - I chose to host on a kubernetes cluster because it is cheaper to run than 
	 - Within the cluster nginx ingress is used by the APIs to access and load balance the traffic to each service
 - I imagine the Api Gateway db would be a relational database so I would probably deploy it as a Postgres Db on AWS Aurora
	 - I chose Aurora over RDS because of the managed scalability
 - I chose dynamodb for the payment/merchant table because it seems like it would be a simple schema making a full relational database overkill
 - I would make the card details table a dynamodb table with the actual card details being encrypted before saving to the table. 