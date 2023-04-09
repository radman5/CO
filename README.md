# RadPay Demo Payment Gateway

### Prerequisites 
1. Visual Studio 2022 (or any with .NET 7 support) 
2. Docker Desktop installed and running
## How to run solution

 1. Ensure debug profile in Visual Studio is set to the "Docker" profile
 2. Click the "Docker" play button

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
 
## Architecture
###  Cloud infrastructure considerations
 - NLB in front of the payment system
	 - Faster than ALB
	 - Can handle traffic spikes better which would be useful during times like Christmas and Black Friday sales
 - All APIs and Worker applications are .NET 7 applications deployed as separate services on a single kubernetes cluster.
	 - I chose to host on a kubernetes cluster because it is cheaper to run than 
	 - Within the cluster nginx ingress is used by the APIs to access and load balance the traffic to each service
 - All databases are Postgres on AWS Aurora
	 - I chose Aurora over RDS because of the managed scalability
	 - Postgres over SQL Server because of the cost
	 - RDBMS over dynamo because I have more experience with it, but can see areas where I might consider it.