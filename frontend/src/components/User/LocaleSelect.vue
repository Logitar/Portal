<template>
  <form-select id="locale" label="user.locale.label" :options="options" placeholder="user.locale.placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getLocales } from '@/api/locales'

export default {
  name: 'LocaleSelect',
  props: {
    value: {}
  },
  data() {
    return {
      locales: []
    }
  },
  computed: {
    options() {
      return this.orderBy(
        this.locales.map(({ code, displayName }) => ({ text: displayName, value: code })),
        'text'
      )
    }
  },
  async created() {
    try {
      const { data } = await getLocales()
      this.locales = data
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
