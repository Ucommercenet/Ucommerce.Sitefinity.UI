<template>
    <div :class="classes">
        <div class="controls">
            <div :class="backBtnClasses">
                <template v-if="showBackButton">
                    <a :href="backUrl" :class="backLinkClasses">Back</a>
                </template>
            </div>
            <div :class="continueBtnClasses">
                <template v-if="showContinueButton">
                    <template v-if="nextStepLink">
                        <a :href="continueUrl">
                            <button type="button" :class="nextLinkClasses">{{ continueLabel }}</button>
                        </a>
                    </template>
                    <template v-else>
                        <input type="submit" :class="nextSubmitClasses" :value="continueLabel" />
                    </template>
                </template>
            </div>
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
                default: 'row control-group multistep-btn-a section-margin'
            },
            mode: {
                type: String,
                default: ''
            }
        },
        computed: {
            continueUrl: function () {
                var url = this.model ? this.model.NextStepUrl : "#";
                return url;
            },
            showContinueButton: function () {
                var url = this.model ? this.model.NextStepUrl : false;
                return Boolean(url);
            },
            continueBtnClasses: function () {
                if (this.showContinueButton && this.showBackButton) {
                    return 'col-md-6 padding-0';
                }
                else if (this.showContinueButton) {
                    return 'col-md-12 padding-0';
                }
            },
            backUrl: function () {
                var url = this.model ? this.model.PreviousStepUrl : "#";
                return url;
            },
            showBackButton: function () {
                var url = this.model ? this.model.PreviousStepUrl : false;
                return Boolean(url);
            },
            backBtnClasses: function () {
                if (this.showBackButton) {
                    return 'col-md-6 padding-0';
                }
            },
            backLinkClasses: function () {
                switch (this.mode) {
                    case 'Bootstrap':
                        return 'btn btn-next btn-transparent pull-left';

                    default:
                        return '';
                }
            },
            nextLinkClasses: function () {
                switch (this.mode) {
                    case 'Bootstrap':
                        return 'btn btn-success btn-next pull-right';

                    default:
                        return '';
                }
            },
            nextSubmitClasses: function () {
                switch (this.mode) {
                    case 'Bootstrap':
                        return 'btn btn-success pull-right submit-address';

                    default:
                        return '';
                }
            }
        }
    };
</script>