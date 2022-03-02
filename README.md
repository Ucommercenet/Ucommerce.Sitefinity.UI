# Ucommerce.Sitefinity.UI

## Compatibility

- Sitefinity 14.0
- Ucommerce 9.5.1

## Intro

To get started with using the Widgets for Ucommerce on Sitefinity you can watch this youtube video: 

https://www.youtube.com/watch?v=vBS7qVVYJtg&t=131s

## Setting up the Widgets in Sitefinity
Each widget requires configuration to function properly. Please refer to the docs below to do so.
https://docs.ucommerce.net/ucommerce/v8.1/sitefinity/setting-up-ucommerce-sitefinity-ui.html

## Vue Migration Notes

The Vue app uses Webpack for build configuration. The main entry point is `index.js` located in the `assets\src` folder

### To run the project in Prod mode
1. CMD into the root of the Ucommerce.Sitefinity.UI project i.e. `Ucommerce.Sitefinity.UI v14.0\Ucommerce.Sitefinity.UI`
2. Run `npm install`
3. Run `npm run build`

### To run the project in Dev mode
1. CMD into the root of the Ucommerce.Sitefinity.UI project
2. Run `npm install`
3. _Recommended_: To speed up development process, you can open "webpack.config.js" and:
	* Comment line **74** `path: path.resolve(__dirname, './assets/build'),` - this will stop the output folder from the class library
	* Uncomment line **73** `//path: path.resolve(__dirname, '../../uquantum/TelerikSitefinitySamplesQuantum/assets/build'),`  - this will output to the root of the UQuantum project. You can change the path form line 73 to match the location of the UQuantum proejct, so you can have constant save and refresh cycle, vs build and restart...
4. Run `npm run dev`

## To build the package locally
`.\Tools\nuget.exe pack .\Ucommerce.Sitefinity.UI\Ucommerce.Sitefinity.UI.nuspec -OutputDirectory C:\_Nuget              `

### Confirmation mail

To use all the benefits of the confirmation email template the page where the widget is dropped must be based on Email Page Template

### Order Confirmation Page
There is a known redirect to the order confirmation page which redirects to `localhost` instead of `localhost:60876`. This is an issue with the Ucommerce core so when testing when you end up on `http://localhost/shop/checkout/confirmation?orderGuid={orderGuid}` you will need to add the port i.e. `http://localhost:60876/shop/checkout/confirmation?orderGuid={orderGuid}`