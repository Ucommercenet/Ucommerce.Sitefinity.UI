<template>
    <div>
        <template v-if="type == 'text'">
            <label>{{ label }}</label>
            <input :id="inputId" :name="inputName" :type="type" :class="inputClasses" :required="required" v-model.lazy="value" @input="handleInput" @change="handleChange">
            <span class="field-validation-error text-danger">{{ errorMessage }}</span>
        </template>
    </div>
</template>
<script>
    export default {
        name: "inputField",
        data: function () {
            return {
                value: null,
                errorMessage: null
            }
        },
        props: {
            model: {
                default: null
            },
            addressType: {
                default: null
            },
            label: {
                default: ''
            },
            fieldName: {
                default: ''
            },
            type: {
                default: 'text'
            },
            required: {
                type: Boolean,
                default: false
            },
            inputClasses: {
                default: ''
            }
        },
        computed: {
            inputId: function () {
                return this.addressType + '_' + this.fieldName;
            },
            inputName: function () {
                return this.addressType + '.' + this.fieldName;
            }
        },
        created: function () {
            this.value = this.model[this.addressType][this.fieldName];
        },
        methods: {
            getValue: function () {
                return this.value;
            },
            handleInput: function () {
                this.errorMessage = '';
            },
            handleChange: function () {
                this.model[this.addressType][this.fieldName] = this.value;

                this.validate((valid, message) => {
                    this.errorMessage = message;
                });
            },
            validate: function (callback) {
                var me = this;
                var value = this.getValue();
                var name = this.inputName;

                if (!this.required && !value) {
                    callback(true, null);
                    return;
                }

                // dummy erquest
                setTimeout(function () {
                    callback(true, null);
                }, 500);

                if (this.model.ValidationUrl) {
                    this.$http.post(model.ValidationUrl,
                        {
                            FieldName: name,
                            FieldValue: value
                        }).then(function (response) {
                            if (response.data) {
                                var valid = response.data.valid;
                                var message = response.data.message;
                                callback(valid, message);
                            }
                        });
                }
            }
        }
    };
</script>
