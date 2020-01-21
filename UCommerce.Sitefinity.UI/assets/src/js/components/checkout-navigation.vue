<template>
    <div v-bind:class="classes">
        <div class="controls">
            <div v-bind:class="backBtnClasses">
                <template v-if="showBackButton">
                    <a v-bind:href="backUrl" class="btn btn-next btn-transparent pull-left">Back</a>
                </template>
            </div>
            <div v-bind:class="continueBtnClasses">
                <template v-if="showContinueButton">
                    <template v-if="nextStepLink">
                        <a v-bind:href="continueUrl">
                            <button type="button" class="btn btn-success btn-next pull-right">{{ continueLabel }}</button>
                        </a>
                    </template>
                    <template v-else>
                        <input type="submit" class="btn btn-success pull-right submit-address" v-bind:value="continueLabel" />
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
            model: null,
            nextStepLink: false,
            continueLabel: 'Continue to next step',
            classes: 'row control-group multistep-btn-a section-margin'
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
            }
        }
    };
</script>