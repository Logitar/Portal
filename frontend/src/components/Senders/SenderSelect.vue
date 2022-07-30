<template>
  <form-select :id="id" :label="label" :options="options" :placeholder="placeholder" :required="required" :value="value" @input="$emit('input', $event)" />
</template>

<script>
import { getSenders } from '@/api/senders'

export default {
  name: 'SenderSelect',
  props: {
    id: {
      type: String,
      default: 'sender'
    },
    label: {
      type: String,
      default: 'senders.select.label'
    },
    placeholder: {
      type: String,
      default: 'senders.select.placeholder'
    },
    realmId: {},
    required: {
      type: Boolean,
      default: false
    },
    value: {}
  },
  data() {
    return {
      senders: []
    }
  },
  computed: {
    options() {
      return this.senders.map(({ id, emailAddress, displayName }) => ({
        text: displayName ? `${displayName} <${emailAddress}>` : emailAddress,
        value: id
      }))
    }
  },
  methods: {
    async refresh(realmId) {
      try {
        const { data } = await getSenders({
          realmId,
          sort: 'EmailAddress',
          desc: false
        })
        this.senders = data.items
      } catch (e) {
        this.handleError(e)
      }
    }
  },
  watch: {
    realmId: {
      immediate: true,
      handler(realmId) {
        this.refresh(realmId)
      }
    }
  }
}
</script>
