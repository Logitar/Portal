<template>
  <validation-provider :name="$t(label).toLowerCase()" :rules="allRules" :vid="id" v-slot="validationContext" slim>
    <b-form-group :label="required ? '' : $t(label)" :label-for="id">
      <template #label v-if="required"><span class="text-danger">*</span> {{ $t(label) }}</template>
      <slot name="before" />
      <b-input-group>
        <b-form-select
          :disabled="disabled"
          :id="id"
          :options="options"
          :state="hasRules ? getValidationState(validationContext) : null"
          :value="value"
          @input="$emit('input', $event)"
        >
          <template #first v-if="placeholder">
            <b-form-select-option :disabled="required" :value="null">{{ $t(placeholder) }}</b-form-select-option>
          </template>
        </b-form-select>
        <slot />
      </b-input-group>
      <div
        v-if="validationContext.errors.length"
        tabindex="-1"
        role="alert"
        aria-live="assertive"
        aria-atomic="true"
        class="invalid-feedback d-block"
        v-text="validationContext.errors[0]"
      />
      <slot name="after" />
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
    options: {
      type: Array,
      default: () => []
    },
    placeholder: {
      type: String,
      default: ''
    },
    required: {
      type: Boolean,
      default: false
    },
    rules: {
      type: Object,
      default: null
    },
    value: {}
  },
  computed: {
    allRules() {
      const rules = this.rules ?? {}
      if (this.required) {
        rules.required = true
      }
      return rules
    },
    hasRules() {
      return Object.keys(this.allRules).length
    }
  }
}
</script>
