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
    realm: {},
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
    async refresh(realm) {
      try {
        const { data } = await getSenders({
          realm,
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
    realm: {
      immediate: true,
      handler(realm) {
        this.refresh(realm)
      }
    }
  }
}
</script>
