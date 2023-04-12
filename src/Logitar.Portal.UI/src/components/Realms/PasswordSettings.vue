<template>
  <div>
    <h5 v-t="'realms.password.title'" />
    <b-row>
      <form-field
        class="col"
        id="requiredLength"
        label="realms.password.requiredLength"
        :minValue="1"
        type="number"
        :value="value.requiredLength"
        @input="onInput({ requiredLength: Number($event) })"
      />
      <form-field
        class="col"
        id="requiredUniqueChars"
        label="realms.password.requiredUniqueChars"
        :minValue="1"
        :maxValue="value.requiredLength"
        type="number"
        :value="value.requiredUniqueChars"
        @input="onInput({ requiredUniqueChars: Number($event) })"
      />
    </b-row>
    <b-form-group>
      <b-form-checkbox id="requireLowercase" :checked="value.requireLowercase" @input="onInput({ requireLowercase: $event })">
        {{ $t('realms.password.requireLowercase') }}
      </b-form-checkbox>
      <b-form-checkbox id="requireUppercase" :checked="value.requireUppercase" @input="onInput({ requireUppercase: $event })">
        {{ $t('realms.password.requireUppercase') }}
      </b-form-checkbox>
      <b-form-checkbox id="requireDigit" :checked="value.requireDigit" @input="onInput({ requireDigit: $event })">{{
        $t('realms.password.requireDigit')
      }}</b-form-checkbox>
      <b-form-checkbox id="requireNonAlphanumeric" :checked="value.requireNonAlphanumeric" @input="onInput({ requireNonAlphanumeric: $event })">
        {{ $t('realms.password.requireNonAlphanumeric') }}
      </b-form-checkbox>
    </b-form-group>
  </div>
</template>

<script>
export default {
  name: 'PasswordSettings',
  props: {
    value: {}
  },
  methods: {
    onInput(changes) {
      const value = { ...this.value }
      for (const [key, change] of Object.entries(changes)) {
        value[key] = change
      }
      this.$emit('input', value)
    }
  }
}
</script>
