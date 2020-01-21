<template>
        <div v-if="showContinueButton" class="control-group margin-top">
            <div class="controls">
                <a v-bind:href="url">
                    <button type="button" class="btn btn-success btn-next pull-right">Continue</button>
                </a>
            </div>
        </div>
</template>
<script>
    export default {
        name: "nextStep",
        props: ['sfmodel'],
        created: function () {
            console.log(this.sfmodel);
        },
        computed: {
            url: function () {
                console.log(this.sfmodel);
                var url = this.sfmodel ? this.sfmodel.NextStepUrl : "#";
                return url;
            },
            showContinueButton: function () {
                var url = this.sfmodel ? this.sfmodel.NextStepUrl : false;
                return Boolean(url);
            }
        }
    };
</script>