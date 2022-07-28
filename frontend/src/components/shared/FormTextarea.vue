<template>
  <validation-provider :name="$t(label).toLowerCase()" :rules="allRules" :vid="id" v-slot="validationContext" slim>
    <b-form-group :label="required ? '' : $t(label)" :label-for="id" :invalid-feedback="validationContext.errors[0]">
      <template #label v-if="required"><span class="text-danger">*</span> {{ $t(label) }}</template>
      <b-form-textarea
        :disabled="disabled"
        :id="id"
        :placeholder="$t(placeholder)"
        :ref="id"
        :rows="rows"
        :state="hasRules ? getValidationState(validationContext) : null"
        :type="type"
        :value="value"
        @input="$emit('input', $event)"
      />
    </b-form-group>
  </validation-provider>
</template>

<script>
import { v4 as uuidv4 } from 'uuid'

export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: () => uuidv4()
    },
    label: {
      type: String,
      default: ''
    },
    maxLength: {
      type: Number,
      default: 0
    },
    minLength: {
      type: Number,
      default: 0
    },
    placeholder: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    rows: {
      type: Number,
      default: 25
    },
    rules: {
      type: Object,
      default: null
    },
    type: {
      type: String,
      default: 'text'
    },
    value: {}
  },
  computed: {
    allRules() {
      const rules = this.rules ?? {}
      if (this.maxLength) {
        rules.max = this.maxLength
      }
      if (this.minLength) {
        rules.min = this.minLength
      }
      if (this.required) {
        rules.required = true
      }
      return rules
    },
    hasRules() {
      return Object.keys(this.allRules).length
    }
  },
  methods: {
    focus() {
      this.$refs[this.id].focus()
    }
  }
}
</script>
