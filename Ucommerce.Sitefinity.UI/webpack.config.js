const path = require('path');
const webpack = require('webpack');
const TerserPlugin = require('terser-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const cssnano = require("cssnano");

let pathsToClean = [
    './assets/build/*'
];

let cleanOptions = {
    root: path.resolve(__dirname, 'Ucommerce.Sitefinity.UI'),
    verbose: true,
    dry: false
};

module.exports = {
    entry: {
        main: ["@babel/polyfill", './assets/src/index.js']
    },
    performance: {
        hints: false
    },
    devtool: 'source-map',
    node: {
        fs: "empty"
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                default: false,
                vendors: false,
                vendor: {
                    test: /[\\/]node_modules|libs[\\/]/,
                    name: "vendor",
                    chunks: 'all'
                },
                styles: {
                    name: 'main',
                    test: /\.(css|scss)$/,
                    chunks: 'all',
                    enforce: true
                }
            }
        },
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    ecma: undefined,
                    warnings: false,
                    parse: {},
                    compress: {},
                    mangle: true,
                    module: false,
                    output: null,
                    toplevel: false,
                    nameCache: null,
                    ie8: false,
                    keep_classnames: undefined,
                    keep_fnames: false,
                    safari10: false
                }
            })
        ]
    },
    output: {
        jsonpFunction: 'jsonpFunction',
        filename: 'js/[name].js',
        chunkFilename: 'js/[name].bundle.js',
        //path: path.resolve(__dirname, '../../uquantum/TelerikSitefinitySamplesQuantum/assets/build'),
        path: path.resolve(__dirname, './assets/build'),
        publicPath: "assets/build/"
    },
    mode: "development",
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.min.js'
        },
        extensions: ['*', '.js', '.vue', '.json']
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'],
                        plugins: ['@babel/plugin-proposal-object-rest-spread', '@babel/plugin-syntax-dynamic-import']
                    }
                }
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: 'css-loader' }
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: 'css-loader' },
                    { loader: 'sass-loader' }
                ]
            },
            {
                test: /\.less$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader'
                    },
                    {
                        loader: 'less-loader'
                    }
                ]
            },
            //{
            //    test: /\.svg$/,
            //    loader: 'svg-sprite-loader',
            //    include: path.resolve(__dirname, '/ResourcePackages/Pekin/assets/src/images/'),
            //    options: {
            //        extract: true
            //    }
            //},
            //{
            //    type: 'javascript/auto',
            //    test: /\.(woff(2)?|ttf|eot|svg|json)$/,
            //    include: [/fonts/, /bootstrap/],
            //    loaders: [{
            //        loader: 'file-loader',
            //        options: {
            //            outputPath: './fonts/',
            //            publicPath: '',
            //            name: '[name].[ext]'
            //        }
            //    }]
            //},
            //{
            //    test: /\.png$/,
            //    exclude: /layout/,
            //    loaders: [{
            //        loader: 'file-loader',
            //        options: {
            //            outputPath: './css/jquery-ui',
            //            publicPath: '../css/jquery-ui',
            //            name: '[name].[ext]'
            //        }
            //    }]
            //},
            //{
            //    test: /\.(jpg|svg|png|gif|ico)$/,
            //    include: [/icons/, /images/, /PortalApp/, /assets/],
            //    exclude: /node_modules/,
            //    loaders: [{
            //        loader: 'file-loader',
            //        options: {
            //            name: '[name].[ext]',
            //            outputPath: './images/',
            //            publicPath: ''
            //        }
            //    }]
            //},
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            }
        ]
    },
    plugins: [
        //new CleanWebpackPlugin(pathsToClean, cleanOptions),
        new CleanWebpackPlugin(),
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin({
            filename: "css/[name].css",
            chunkFilename: "css/[name].css",
            publicPath: "../css/jquery-ui"
        }),
        new OptimizeCssAssetsPlugin({
            assetNameRegExp: /\.css$/g,
            cssProcessor: cssnano,
            cssProcessorPluginOptions: {
                preset: ['default', { discardComments: { removeAll: true } }]
            },
            canPrint: true
        })
    ]
};