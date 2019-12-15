# APP
The app scrapes data from a URL, stats on words are expressed in a word cloud.
* Concepts employed:
    * web scraping
    * removing stopwords
    * some basic semantic rules on checking actual English words


### technologies 
---
* .Net Core - the app was developed using platform agnostic version of .Net. As an example, this mac was developed on the mac OS platform

* Knockout - JS technology for easier data binding. Uses MVVM pattern

### description
---
The app saves these dictionary items in a database. Data is encrypted for security purposes

### Encryption key management
---
***IV*** key can be safely stored in the database, alongside the salt

***Encryption Key*** can be stored in any file or registry key, as long as it is not remotely accessible

### setting up
---

* change db connection config according to your setup in StartUp.cs

* load sample sql database found in *root of app -> wwwroot -> sql*

* Follow the hosting instruction under [Host and deploy](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1)




