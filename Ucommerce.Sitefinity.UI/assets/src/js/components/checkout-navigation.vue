<template>
    <div v-if="model" :class="classes">
        <div :class="backWrapperClasses">
            <template v-if="showBackButton">
                <a :href="backUrl" :class="backLinkClasses">Back</a>
            </template>
        </div>
        <div :class="continueWrapperClasses">
            <template v-if="showContinueButton">
                <button type="button" :class="continueBtnClasses" @click="continueHandler">{{ continueLabel }}</button>
            </template>
        </div>
    </div>
</template>
<script>
    export default {
        name: "checkoutNavigation",
        props: {
            model: {
                default: null
            },
            nextStepLink: {
                type: Boolean,
                default: false
            },
            continueLabel: {
                type: String,
                default: 'Continue'
            },
            classes: {
                type: String,
                default: ''
            },
            backLinkClasses: {
                type: String,
                default: ''
            },
            continueBtnClasses: {
                type: String,
                default: ''
            },
            mode: {
                type: String,
                default: ''
            },
            continueFn: {
                type: Function,
                default: null
            },
            allowNavigate: {
                default: true
            }
        },
        watch: {
            allowNavigate: function (allow) {
                console.log(allow);
                if (allow) {
                    location.href = this.model.NextStepUrl;
                }
            }
        },
        computed: {
            backUrl: function () {
                var url = this.model ? this.model.PreviousStepUrl : "#";
                return url;
            },
            continueUrl: function () {
                var url = this.model ? this.model.NextStepUrl : "#";
                return url;
            },
            showBackButton: function () {
                var url = this.model ? this.model.PreviousStepUrl : false;
                return Boolean(url);
            },
            showContinueButton: function () {
                var url = this.model ? this.model.NextStepUrl : false;
                return Boolean(url);
            },
            backWrapperClasses: function () {
                if (this.showBackButton && (this.mode == 'Bootstrap' || this.mode == 'Bootstrap4')) {
                    return 'col-md-6 padding-0';
                }
                else {
                    return '';
                }
            },
            continueWrapperClasses: function () {
                if (this.mode == 'Bootstrap' || this.mode == 'Bootstrap4') {
                    if (this.showContinueButton && this.showBackButton) {
                        return 'col-md-6  padding-0';
                    }
                    else if (this.showContinueButton) {
                        return 'col-md-12 clear padding-0';
                    }
                }
                else {
                    return '';
                }
            },
            backLinkClasses: function () {
                switch (this.mode) {
                    case 'Bootstrap':
                        return 'btn btn-back btn-transparent pull-left';

                    case 'Bootstrap4':
                        return '';

                    default:
                        return '';
                }
            }
        },
        methods: {
            continueHandler: function () {
                if (this.continueFn) {
                    this.continueFn((allowRedirect) => {
                        if (allowRedirect) {
                            location.href = this.model.NextStepUrl;
                        }
                    });
                    return;
                }

                location.href = this.model.NextStepUrl;
            }
        }
    };
</script>
