# Ucommerce.Sitefinity.UI

## Intro

To get started with using the Widgets for Ucommerce on Sitefinity you can watch this youtube video: 

https://www.youtube.com/watch?v=vBS7qVVYJtg&t=131s

## Vue Migration Notes

The Vue app uses Webpack for build configuration. The main entry point is `index.js` located in the `assets\src` folder

### To run the project in Prod mode
1. CMD into the root of the Ucommerce.Sitefinity.UI project
2. Run `npm install`
3. Run `npm run build`

### To run the project in Dev mode
1. CMD into the root of the Ucommerce.Sitefinity.UI project
2. Run `npm install
3. _Recommended_: To speed up development process, you can open "webpack.config.js" and:
	* Comment line **74** `path: path.resolve(__dirname, './assets/build'),` - this will stop the output folder from the class library
	* Uncomment line **73** `//path: path.resolve(__dirname, '../../uquantum/TelerikSitefinitySamplesQuantum/assets/build'),`  - this will output to the root of the UQuantum project. You can change the path form line 73 to match the location of the UQuantum proejct, so you can have constant save and refresh cycle, vs build and restart...
4. Run `npm run dev`

### Confirmation mail

To use all the benefits of the confirmation email template the page where the widget is dropped must be based on Email Page Template
