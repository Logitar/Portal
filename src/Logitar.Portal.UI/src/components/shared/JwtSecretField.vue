<template>
  <b-form-group>
    <form-field
      :id="id"
      :label="label"
      :minLength="minLength"
      :maxLength="maxLength"
      :placeholder="placeholder"
      :type="showSecret ? 'text' : 'password'"
      :value="value"
      @input="$emit('input', $event)"
    >
      <b-input-group-append>
        <icon-button :icon="showSecret ? 'eye-slash' : 'eye'" variant="info" @click="showSecret = !showSecret" />
        <icon-button :disabled="!value" icon="times" text="jwt.secret.clear" variant="warning" @click="$emit('input', null)" />
      </b-input-group-append>
    </form-field>
    <b-alert :show="oldValue && oldValue !== value" variant="warning">
      <p><strong v-t="warning" /></p>
      <icon-button icon="history" text="jwt.secret.revert" variant="warning" @click="$emit('input', oldValue)" />
    </b-alert>
  </b-form-group>
</template>

<script>
export default {
  name: 'JwtSecretField',
  props: {
    id: {
      type: String,
      default: 'secret'
    },
    label: {
      type: String,
      default: 'jwt.secret.label'
    },
    maxLength: {
      type: Number,
      default: 512 / 8
    },
    minLength: {
      type: Number,
      default: 256 / 8
    },
    oldValue: {},
    placeholder: {
      type: String,
      default: 'jwt.secret.placeholder'
    },
    value: {},
    warning: {
      type: String,
      default: 'jwt.secret.warning'
    }
  },
  data() {
    return {
      showSecret: false
    }
  }
}
</script>
