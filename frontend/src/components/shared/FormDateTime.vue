<template>
  <validation-provider :name="$t(label).toLowerCase()" :rules="rules" :vid="id" v-slot="validationContext" slim>
    <b-form-group :label="required ? '' : $t(label)" :label-for="id" :invalid-feedback="validationContext.errors[0]">
      <template #label v-if="required"><span class="text-danger">*</span> {{ $t(label) }}</template>
      <b-form-input
        :disabled="disabled"
        :id="id"
        :max="validate ? max : null"
        :min="validate ? min : null"
        :ref="id"
        :state="validate ? getValidationState(validationContext) : null"
        type="datetime-local"
        :value="formattedValue"
        @input="$emit('input', $event ? new Date($event) : null)"
      />
      <slot />
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
    maxDate: {
      type: Date
    },
    minDate: {
      type: Date
    },
    required: {
      type: Boolean,
      default: false
    },
    validate: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  computed: {
    formattedValue() {
      if (typeof this.value === 'undefined' || this.value === null) {
        return null
      }
      return this.getDatetimeLocal(this.value)
    },
    max() {
      if (typeof this.maxDate === 'undefined' || this.maxDate === null) {
        return null
      }
      return this.getDatetimeLocal(this.maxDate)
    },
    min() {
      if (typeof this.minDate === 'undefined' || this.minDate === null) {
        return null
      }
      return this.getDatetimeLocal(this.minDate)
    },
    rules() {
      return {
        required: this.required
      }
    }
  },
  methods: {
    getDatetimeLocal(value) {
      const instance = value instanceof Date ? value : new Date(value)
      const date = [instance.getFullYear(), (instance.getMonth() + 1).toString().padStart(2, '0'), instance.getDate().toString().padStart(2, '0')].join('-')
      const time = [instance.getHours().toString().padStart(2, '0'), instance.getMinutes().toString().padStart(2, '0')].join(':')
      return [date, time].join('T')
    }
  }
}
</script>
