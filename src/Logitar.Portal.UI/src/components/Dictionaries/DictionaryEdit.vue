<template>
  <b-container>
    <h1 v-t="dictionary ? 'dictionaries.editTitle' : 'dictionaries.newTitle'" />
    <status-detail v-if="dictionary" :model="dictionary" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="dictionary" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <b-row>
          <realm-select class="col" :disabled="Boolean(dictionary)" v-model="selectedRealm" />
          <locale-select class="col" :disabled="Boolean(dictionary)" required v-model="selectedLocale" />
        </b-row>
        <h3 v-t="'dictionaries.entries.label'" />
        <div class="my-2">
          <icon-button icon="plus" text="dictionaries.entries.add" variant="success" @click="addEntry" />
        </div>
        <b-row v-for="(entry, index) in entries" :key="index">
          <form-field
            class="col"
            hideLabel
            :id="`key_${index}`"
            label="dictionaries.entries.key"
            :maxLength="256"
            placeholder="dictionaries.entries.key"
            required
            :rules="{ identifier: true }"
            :value="entry.key"
            @input="setEntry(index, $event, entry.value)"
          />
          <form-field
            class="col"
            hideLabel
            :id="`value_${index}`"
            label="dictionaries.entries.value"
            placeholder="dictionaries.entries.value"
            required
            :value="entry.value"
            @input="setEntry(index, entry.key, $event)"
          >
            <icon-button class="mx-3" icon="times" variant="danger" @click="removeEntry(index)" />
          </form-field>
        </b-row>
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import Vue from 'vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { createDictionary, updateDictionary } from '@/api/dictionaries'

export default {
  name: 'DictionaryEdit',
  components: {
    RealmSelect
  },
  props: {
    json: {
      type: String,
      default: ''
    },
    locale: {
      type: String,
      default: ''
    },
    realm: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      dictionary: null,
      entries: [],
      loading: false,
      selectedLocale: null,
      selectedRealm: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.dictionary && this.selectedRealm) ||
        (!this.dictionary && this.selectedLocale) ||
        JSON.stringify(this.entries) !== JSON.stringify(this.dictionary?.entries ?? [])
      )
    },
    payload() {
      const payload = {
        entries: [...this.entries]
      }
      if (!this.dictionary) {
        payload.locale = this.selectedLocale
        payload.realm = this.selectedRealm
      }
      return payload
    }
  },
  methods: {
    addEntry() {
      this.entries.push({ key: null, value: null })
    },
    removeEntry(index) {
      Vue.delete(this.entries, index)
    },
    setEntry(index, key, value) {
      Vue.set(this.entries, index, { key, value })
    },
    setModel(dictionary) {
      this.dictionary = dictionary
      this.selectedLocale = dictionary.locale
      this.selectedRealm = dictionary.realm?.id ?? null
      this.entries = this.orderBy([...dictionary.entries], 'key')
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            if (this.dictionary) {
              const { data } = await updateDictionary(this.dictionary.id, this.payload)
              this.setModel(data)
              this.toast('success', 'dictionaries.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createDictionary(this.payload)
              window.location.replace(`/dictionaries/${data.id}?status=created`)
            }
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.locale) {
      this.selectedLocale = this.locale
    }
    if (this.realm) {
      this.selectedRealm = this.realm
    }
    if (this.status === 'created') {
      this.toast('success', 'dictionaries.created')
    }
  }
}
</script>
