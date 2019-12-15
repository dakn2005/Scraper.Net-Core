# APP
The app scrapes data from a URL, stats on words are expressed in a word cloud.
* Concepts employed:
    * web scraping
    * removing stopwords
    * some basic semantic rules on checking actual English words

### description
---
The app saves these dictionary items in a database. Data is encrypted for security purposes

### Encryption key management
---
***IV*** key can be safely stored in the database, alongside the salt

***Encryption Key*** can be stored in any file or registry key, as long as it is not remotely accessible


