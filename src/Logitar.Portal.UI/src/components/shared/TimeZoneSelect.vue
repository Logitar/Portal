<template>
  <form-select
    :disabled="disabled"
    :id="id"
    :label="label"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :value="value"
    @input="$emit('input', $event)"
  />
</template>

<script>
import { getTimeZones } from '@/api/timeZones'

export default {
  name: 'TimeZoneSelect',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    id: {
      type: String,
      default: 'timeZone'
    },
    label: {
      type: String,
      default: 'timeZone.label'
    },
    placeholder: {
      type: String,
      default: 'timeZone.placeholder'
    },
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data() {
    return {
      timeZones: []
    }
  },
  computed: {
    options() {
      return this.orderBy(
        this.timeZones.map(({ id, displayName }) => ({ text: displayName, value: id })),
        'text'
      )
    }
  },
  async created() {
    try {
      const { data } = await getTimeZones()
      this.timeZones = data
    } catch (e) {
      this.handleError(e)
    }
  }
}
</script>
