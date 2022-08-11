<template>
  <form-select :id="id" :label="label" :options="options" :placeholder="placeholder" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getLocales } from '@/api/locales'

export default {
  name: 'LocaleSelect',
  props: {
    id: {
      type: String,
      default: 'locale'
    },
    label: {
      type: String,
      default: 'locale.label'
    },
    placeholder: {
      type: String,
      default: 'locale.placeholder'
    },
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
