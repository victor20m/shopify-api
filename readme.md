# Shopify App

ASP.NET Core API to integrate shopify store.


## Configuration

```python
appsettings.json

#Add this config
"ShopifyDbContext": {
    "ConnectionString": string,
    "DatabaseName": string,
    "CustomersCollection": string
  }
```

```python
#Manage secrets -> secrets.json

"shopify": {
    "api_key",
    "api_secret",
    "client_secret",
    "store_url"
  }
```

```python
#ShopifyApp/Scripts/secrets.json

"shopify": {
    "api_key",
    "api_secret",
    "client_secret",
    "store_url"
  }
```
