<template>
  <b-form-group :label="required || hideLabel ? '' : $t(label)" :label-for="id">
    <template #label v-if="required && !hideLabel"><span class="text-danger">*</span> {{ $t(label) }}</template>
    <slot name="before" />
    <b-input-group>
      <vue-phone-number-input
        :class="{ input: true, verified }"
        :default-country-code="value.countryCode"
        :id="id"
        :error="!isValid"
        error-color="#dc3545"
        :ref="id"
        :required="required"
        :translations="translations"
        valid-color="#28a745"
        v-model="phoneNumber"
        @update="onUpdate"
      />
      <b-input-group-append v-if="verified">
        <b-input-group-text class="bg-info text-white"><font-awesome-icon icon="check" />&nbsp;{{ $t('user.phone.verified') }}</b-input-group-text>
      </b-input-group-append>
      <slot />
    </b-input-group>
    <div v-if="!isValid" tabindex="-1" role="alert" aria-live="assertive" aria-atomic="true" class="invalid-feedback d-block" v-t="'user.phone.invalid'" />
  </b-form-group>
</template>

<script>
export default {
  name: 'PhoneField',
  props: {
    hideLabel: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'phoneNumber'
    },
    label: {
      type: String,
      default: 'user.phone.number.label'
    },
    placeholder: {
      type: String,
      default: 'user.phone.number.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {
      type: Object,
      required: true
    },
    verified: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      isValid: false,
      phoneNumber: null
    }
  },
  computed: {
    translations() {
      return {
        countrySelectorLabel: this.$i18n.t('user.phone.countryCode.label'),
        countrySelectorError: this.$i18n.t('user.phone.countryCode.placeholder'),
        phoneNumberLabel: this.$i18n.t(this.value.number ? this.label : this.placeholder),
        example: this.$i18n.t('user.phone.number.example')
      }
    }
  },
  methods: {
    onUpdate($event) {
      const { countryCode, isValid, phoneNumber } = $event
      this.isValid = isValid || !phoneNumber

      const value = { ...this.value }
      value.countryCode = countryCode ?? null
      value.number = phoneNumber ?? null
      this.$emit('input', value)
    }
  },
  created() {
    this.phoneNumber = this.value.number ?? null
  }
}
</script>

<style scoped>
.input {
  flex: 1 1 auto;
}

.verified :deep(input[type='tel']) {
  border-top-right-radius: 0 !important;
  border-bottom-right-radius: 0 !important;
}
</style>
