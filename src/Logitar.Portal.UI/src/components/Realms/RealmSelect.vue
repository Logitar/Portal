<template>
  <form-select
    :disabled="disabled"
    id="realm"
    label="realms.select.label"
    :options="options"
    placeholder="realms.select.placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { getRealms } from '@/api/realms'

export default {
  name: 'RealmSelect',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data() {
    return {
      realms: []
    }
  },
  computed: {
    options() {
      return this.realms.map(({ id, name }) => ({ text: name, value: id }))
    }
  },
  async created() {
    try {
      const { data } = await getRealms({ sort: 'Name', desc: false })
      this.realms = data.items
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
