# DatingApp
Dating App made with .NET Core 3.0 Web API and Angular 8 front-end. It is full of functionality.

<p align="center"><img src="https://user-images.githubusercontent.com/48388060/83612534-d1a7fb80-a582-11ea-985a-d96c7971294b.png" width="800" /></p>

## Back-end
Made with .NET Core 3.0 and SQL Server database using Code First approach. SQLite is added for development purposes.
* ### Photo upload
Users can upload photos and store them in a cloud provider - Cloudinary. You can pick your main photo.

* ### Likes
Users can like each other.

* ### Messaging
Users can write messages to each other. The Messaging system is done with classic Inbox/Outbox style. Potential future improvement is to add Real-Time Chat functionality.

* ### Pagination and filters
Pagination is created on the server-side and crucial data about current page/total pages etc. is sent via a header to the method caller. Filters are applied via query params.

* ### Authentication
Application was done with JWT Authentication in mind. Backend is generating a token and expecting it with incoming requests . Later, ASP.NET Identity was added to provide a few roles. Admin panel was created for editing user roles and approving/rejecting photos. 

## Front-end
Front-end has been created with Angular version 8 and Bootstrap 4. Several components are using ngx-bootstrap. Notifications are provided by Alertify js.

<p align="center"><img src="https://user-images.githubusercontent.com/48388060/83611640-8b05d180-a581-11ea-9cb0-ce7993e732c7.png" width="800" /></p>

## Tests
Unit tests are provided for back-end using xUnit framework.

