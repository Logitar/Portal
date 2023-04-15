<template>
  <form-select id="gender" label="user.gender.label" :options="options" placeholder="user.gender.placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
export default {
  name: 'GenderSelect',
  props: {
    value: {}
  },
  computed: {
    options() {
      return this.orderBy(
        Object.entries(this.$i18n.t('user.gender.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  }
}
</script>
