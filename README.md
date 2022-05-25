![image](https://user-images.githubusercontent.com/20577566/170269252-1401798a-fc2f-4881-b4c4-403b137efa06.png)

The picture above shows a summary of the api from swagger.
The solution has been developed in Visual Studio 2019 and uses .Net 5. 
In order to run it you would have to use .Net 5 since in .Net 6 there are some changes like the merging of the Startup and Program files into one file that will probably
make it impossible to run without changes.
This is an implementation of an api for getting data on voyage prices. Besides the 2 endpoints described in the problem text, I have decided to add 3 more endpoins for 
various reasons.
The extra endpoints are as you can see from the attached picture:
- /api/Login for the login functionality 
- /api/Voyages/GetAll for getting all elements 
- /api/Voyages/UpdatePrice for doing the same as in the UpdatePrice defined in the text but using the FromBody to pass the data

I have used a basic JWT token for authentication against some users that are stored in the UserConstants file. The are also different roles used for authentication (Administrator and Normal).
The endpoints are configured as follows:
- GettAll can be used without authentication
- GetAveragePrice can be used by both roles
- both UpdatePrice can only be used only by the  Administrator role.

For testing you can use either admin and admin_PW or normal and normal_PW.
The workflow is as follows, go to login endpoint and used the provided credentials, you will get a token back that you then have to use in the subsequent requests for all endpoints besides GetAll.
If you use Swagger you will have to copy the token into the Authorize form that pops up when you press the Authorize button in the upper right corner (see first picture) and press the authorize button, see picture bellow.

![image](https://user-images.githubusercontent.com/20577566/170269517-50744aab-9883-4dc0-9db5-96d3c2ed4a1d.png)

The solution implements a basic caching strategy for faster access to data.

For the database we store the items in a local json file, I have created an interface called IVoyageDataStore which will make the changing of the data store easy for future use.

Integration tests can be found in the VoyagesApi.IntegrationTests and the unit tests can be found in the VoyagesApi.Tests.

