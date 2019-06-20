# Quantum Demo Installation Instructions

## Goals
Provide setup instructions for the Quantum Demo website showcasing how the Ucommerce.Sitefinity.UI integrates with Sitefinity

## Folder Contents

* DBs - This folders contains the database backups for both the Ucommerce "KenticoDemo" database and the "Quantum" demo site running on Sitefinity
* NuGet Local Source - contains the `Ucommerce.Sitefinity.UI.0.8.0.nupkg` NuGet package that is used as dependency for the Quantum website
* TelerikSitefinitySamplesQuantum - this folder contains the Quantum demo website
* QuantumWebApp.sln - solution file for the Quantum Project

## Installation Instructions

> **NOTE:** This is not a ReadMe file for setting up the "Ucommerce.Sitefinity.UI" project. This is only to setup the demo of it as a NuGet package within the Quantum demo website. For the source code of the "Ucommerce.Sitefinity.UI" project go to the [https://bitbucket.org/athracian/ucommerce-ui/src/master/](https://bitbucket.org/athracian/ucommerce-ui/src/master/)

1. Open the solution in Visual Studio using the "QuantumWebApp.sln" solution file
2. Add the "NuGet Local Source" folder to the list of your NuGet sources - this is where the Ucommerce.Sitefinity.UI is going to be resolved from
3. Restore the NuGet Packages for the solution
4. Restore the "Ucommerce0615.bak" database. The file is located in the "DBs" folder
5. Restore the "KenticoDemo0615.bak" database. The file is located in the "DBs" folder
6. Configure the DataConfig.config, located in App_Data/Sitefinity/Configuration folder. Make sure they point to the Quantum database restored in step 4
```
    <add connectionString="Data Source=.\SqlServer;Integrated Security=SSPI;Initial Catalog=QUantumCommerce" name="Sitefinity" />
    <add connectionString="Data Source=.\SqlServer;Integrated Security=SSPI;Initial Catalog=QUantumCommerce" name="SitefinityDiagnosticsModule" />
```
7. Modify the web.config file and make sure the Ucommerce connection string points to database restored in step 5

``` <add connectionString="Data Source=.\SqlServer;Integrated Security=SSPI;Initial Catalog=KenticoDemo1" name="Ucommerce" />
```

8. Build your solution and make sure it compiles with no errors
    * If you see issue with, please run the "Could not find file '{You Path Here}\TelerikSitefinitySamplesQuantum\bin\roslyn\csc.exe'." error message, please run the following command in the NuGet package manager  `Update-Package -Id Microsoft.CodeDom.Providers.DotNetCompilerPlatform -Reinstall`
9. Run the website
10. When asked you will need to apply a valid license for Sitefinity 11.2.6900
11. Enjoy!

## Credentials

* Username: admin
* Password: admin@2


## Details for the Demo

### Widgets
* Facet Filter widgets - provider facets filter for list of products
* Products widget
    * Displays list of manually or automatically selected products
    * Displays product details
* Category Navigation widget - provides convenient way for users to browse the products catalog based on the defined Categories

### Pages
Below are the list of pages from the Quantum demo website, which can be browser and edited for showcasing the current set of functionalities that the Ucommerce.Sitefinity.UI provides

* Product Catalog Page with manually selected products - "/shop/products"
* Product Catalog Page with Category Navigation driven selection of products "/shop/products/categories/{categoryName}", i.e. "/shop/products/categories/Shoes"
* Product details page with better Urls -"/shop/products/details/{CategoryName}/{ProductSku}", i.e. "/shop/products/details/Shoes/178"