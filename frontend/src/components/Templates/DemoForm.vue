<template>
  <div>
    <b-alert v-if="message" dismissible :variant="resultVariant" v-model="showResult">
      <strong>{{ $t('templates.statusFormat', { status }) }}</strong>
      {{ $t(`templates.status.${status}`) }}
      <b-link :href="`/messages/${message.id}`" target="_blank">{{ $t('templates.result.viewDetail') }} <font-awesome-icon icon="external-link-alt" /></b-link>
    </b-alert>
    <div class="my-2">
      <icon-button :disabled="loading" icon="paper-plane" :loading="loading" text="templates.sendToMe" variant="primary" @click="sendDemo" />
      {{ $t('templates.senderFormat') }}
      <b-link v-if="sender.displayName" :href="`/senders/${sender.id}`" target="_blank">
        {{ sender.displayName }} &lt;{{ sender.emailAddress }}&gt; <font-awesome-icon icon="external-link-alt"
      /></b-link>
      <b-link v-else :href="`/senders/${sender.id}`" target="_blank">{{ sender.emailAddress }} <font-awesome-icon icon="external-link-alt" /></b-link>.
    </div>
    <h3 v-t="'templates.variables.label'" />
    <div class="my-2">
      <icon-button icon="plus" text="templates.variables.add" variant="success" @click="addVariable" />
    </div>
    <b-row v-for="(variable, index) in variables" :key="index">
      <form-field
        class="col"
        :id="`key_${index}`"
        label="templates.variables.key"
        :maxLength="256"
        placeholder="templates.variables.key"
        :rules="{ identifier: true }"
        hideLabel
        :value="variable.key"
        @input="setVariable(index, $event, variable.value)"
      />
      <form-field
        class="col"
        :id="`value_${index}`"
        label="templates.variables.value"
        placeholder="templates.variables.value"
        hideLabel
        :value="variable.value"
        @input="setVariable(index, variable.key, $event)"
      >
        <icon-button class="mx-3" icon="times" variant="danger" @click="removeVariable(index)" />
      </form-field>
    </b-row>
  </div>
</template>

<script>
import Vue from 'vue'
import { isIdentifier } from '@/helpers/stringUtils'
import { sendDemoMessage } from '@/api/messages'

export default {
  name: 'DemoForm',
  props: {
    sender: {
      type: Object,
      required: true
    },
    template: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      message: null,
      showResult: false,
      variables: []
    }
  },
  computed: {
    resultVariant() {
      switch (this.status) {
        case 'Failed':
          return 'danger'
        case 'Sent':
          return 'success'
        case 'Unsent':
          return 'warning'
      }
      return ''
    },
    status() {
      return this.message.succeeded ? 'Sent' : this.message.hasErrors ? 'Failed' : 'Unsent'
    }
  },
  methods: {
    addVariable() {
      this.variables.push({ key: null, value: null })
    },
    removeVariable(index) {
      Vue.delete(this.variables, index)
    },
    setVariable(index, key, value) {
      Vue.set(this.variables, index, { key, value })
    },
    async sendDemo() {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await sendDemoMessage({
            templateId: this.template.id,
            variables: this.variables.filter(({ key }) => isIdentifier(key))
          })
          this.message = data
          this.showResult = true
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
